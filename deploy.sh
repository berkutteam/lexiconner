#!/bin/bash

# use the unofficial bash strict mode. This causes bash to behave in a way that makes many classes of subtle bugs impossible.
# http://redsymbol.net/articles/unofficial-bash-strict-mode/
set -euo pipefail


usage()
{
    cat <<END
deploy.sh: deploys application to Heroku
Parameters:
-environment
    E.g. DevelopmentHeroku, ProductionHeroku
-h | --help
    Displays this help text and exits the script
END
}

# local variables and constants
container_registry="registry.heroku.com"

# should be passed as argument
environment=""
# build_id="" # Azure DevOps buildId
heroku_app_name_api=""
heroku_app_name_identity=""
heroku_app_name_web=""
container_registry_user=""
container_registry_password=""

# read command line arguments
while [[ $# -gt 0 ]]; do
  case "$1" in
    --environment )
        environment="$2"; shift 2 ;;
    --heroku-app-name-api )
        heroku_app_name_api="$2"; shift 2 ;;
    --heroku-app-name-identity )
        heroku_app_name_identity="$2"; shift 2 ;;
    --heroku-app-name-web )
        heroku_app_name_web="$2"; shift 2 ;;
    --container-registry-user )
        container_registry_user="$2"; shift 2 ;;
    --container-registry-password )
        container_registry_password="$2"; shift 2 ;;
    -h | --help )
        usage; exit 1 ;;
    *)
        echo "Unknown option $1"
        usage; exit 2 ;;
  esac
done


# CLI input validations
if [[ ! $environment ]]; then
    echo "'environment' must be specified"
    echo ''
    usage
    exit 3
fi
if [[ ! $heroku_app_name_api ]]; then
    echo "'heroku-app-name-api' must be specified"
    echo ''
    usage
    exit 3
fi
if [[ ! $heroku_app_name_identity ]]; then
    echo "'heroku-app-name-identity' must be specified"
    echo ''
    usage
    exit 3
fi
if [[ ! $heroku_app_name_web ]]; then
    echo "'heroku-app-name-web' must be specified"
    echo ''
    usage
    exit 3
fi


echo "#################### Environment info ####################"
echo "Node version:"
node -v
printf "\n"
printf "\n"


echo "#################### Instaling Heroku CLI... ####################"

if [[ "$OSTYPE" == "linux-gnu" ]]; then
    echo "Running on Linux."
    curl https://cli-assets.heroku.com/install.sh | sh
elif [[ "$OSTYPE" == "cygwin" || "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
    # cygwin: POSIX compatibility layer and Linux environment emulation for Windows
    # msys: Lightweight shell and GNU utilities compiled for Windows (part of MinGW)
    # win32: not sure this can happen.
    echo "Running on Windows."
    echo "Do not install Heroku CLI as it supposed that script is being run on local dev machine."
    echo "Check Heroku CLI version (check installed)."
    heroku --version
else
    echo "Running on '$OSTYPE' which is not supported."
    exit 1;
fi
printf "\n"
printf "\n"


echo "#################### Building SPA projects... ####################"
cd Lexiconner/Lexiconner.Web/vue-client
echo "Instaling NPM packages..."
npm install
echo "Building..."
npm run build_${environment} 
echo "wwwroot files:"
ls ../wwwroot
cd ../../..
printf "\n"
printf "\n"

echo "#################### Building and pushing Docker images... ####################"
docker login --username=$container_registry_user --password=$container_registry_password $container_registry

# service name list
services=(
    $heroku_app_name_api
    $heroku_app_name_identity
    $heroku_app_name_web
)

# service Dockerfile path list
# NB: must match 'services'
serviceDockerfilePathList=(
    Lexiconner/Lexiconner.Api/Dockerfile.ci
    Lexiconner/Lexiconner.IdentityServer4/Dockerfile.ci
    Lexiconner/Lexiconner.Web/Dockerfile.ci
)

service=""
for i in "${!services[@]}"
do
    service=${services[i]}
    dockerfile=${serviceDockerfilePathList[i]}
    echo "Building image for service '$service'..."
    # for Heroku image should be: registry.heroku.com/<app-name>/web
    docker build -f $dockerfile -t $container_registry/$service/web .

    # Remove itermediate images to save disk space
    echo "Cleaning up..."

    ## Option 1: list dangling images and pass to remove command (2 commands)
    # docker rmi --force $(docker images --quiet --filter "dangling=true")

    ## Options 2: remove dangling images (1 command)
    # NB: --all removes all unused images that aren't used in active containers (including images that we built earlier)
    docker image prune --force

    echo "Pushing image for service '$service'..."
    docker push $container_registry/$service/web
done
printf "\n"
printf "\n"


echo "#################### Releasing... ####################"

# create _netrc whick used for Heroku CLI login
netrc_file_path=""
if [[ "$OSTYPE" == "linux-gnu" ]]; then
    netrc_file_path="~/.netrc"
elif [[ "$OSTYPE" == "cygwin" || "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
    netrc_file_path="$HOME/_netrc"
fi
echo "Running on Linux."
echo "Create Heroku .netrc file."
content="
machine api.heroku.com
    login $container_registry_user
    password $container_registry_password
machine git.heroku.com
    login $container_registry_user
    password $container_registry_password
"
#echo $content >> $netrc_file_path
cat >> $netrc_file_path << EOL
machine api.heroku.com
  login $container_registry_user
  password $container_registry_password
machine git.heroku.com
  login $container_registry_user
  password $container_registry_password
EOL

for i in "${!services[@]}"
do
    service=${services[i]}
    dockerfile=${serviceDockerfilePathList[i]}
    echo "Heroku release '$service'..."
    heroku container:release web -a $service
done
printf "\n"
printf "\n"

echo "#################### Finished ####################"
