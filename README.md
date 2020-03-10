# lexiconner
The project is aimed to help save and study words.

## DevelopmentLocalhost

- Api -      https://localhost:5006;http://localhost:5005
- Identity - https://localhost:5004;http://localhost:5003
- Web -      https://localhost:5008;http://localhost:5007

## DevelopmentHeroku

- Api -      https://lexiconner-api-dev.herokuapp.com
- Identity - https://lexiconner-identity-dev.herokuapp.com
- Web -      https://lexiconner-web-dev.herokuapp.com

## ProductionHeroku

- Api -      https://lexiconner-api.herokuapp.com
- Identity - https://lexiconner-identity.herokuapp.com
- Web -      https://lexiconner-web.herokuapp.com


## Spec
- Each study word, card, etc named as 'StudyItem'.
- Db is MongoDb. Used Atlas free tier with 0.5 GB storage.
- Sensitive information is stored in .env file for DevelopmentLocalhost (use DotNetEnv to load from file) and in env variables for DevelopmentHeroku, ProductionHeroku
- Hosted on Heroku
- CI v1: CircleCI (free tier has limited resources)
- CI v2: AzureDevOps public project

## Heroku
- On heroku Docker EXPOSE doesn't work, so you need to use $PORT env variable to listen proper port
The web process must listen for HTTP traffic on $PORT, which is set by Heroku. EXPOSE in Dockerfile is not respected, but can be used for local testing. Only HTTP requests are supported.
- RUNTIME_ENV stores environment where app is running (local, heroku, azure, ...) 
- in Program.cs use .UseUrls and $PORT to setup listening port

## Resources:
 - https://cloud.mongodb.com
 - https://dashboard.heroku.com
 - https://circleci.com
 - https://dev.azure.com (AzureDevOps)

# Azure Dev Ops
- Use env variables in format: $(name), because $name, ${name} doesn't work
- Name variables in uppercase
- In string use $(name) format in code $name
- In YAML use $(name) format.

## Overiview
- Azure DevOps Pipeline: Builds Docker images and pushes them to registry.heroku.com. These steps are defined in `azure-pipelines.yml`.
- Azure DevOps Release: Logins to heroku and puts Heroku app in a release mode.

## Explained steps

### Pipeline for Build

Pipelines -> Pipelines -> New pipeline.

Connect project repository from GitHub. `azure-pipelines.yml` is picked up automatically from repository root.

Setup env varibles:
- `HEROKU_USERNAME`
- `HEROKU_API_KEY` (secret)
- `HEROKU_APP_NAME_API`
- `HEROKU_APP_NAME_IDENTITY`
- `HEROKU_APP_NAME_WEB`

In `azure-pipelines.yml` setup Docker build, tag and publish steps.

### Pipeline for Release

Connect with build pipeline.

Setup env varibles:
- `HEROKU_USERNAME`
- `HEROKU_API_KEY` (secret)
- `HEROKU_APP_NAME_API`
- `HEROKU_APP_NAME_IDENTITY`
- `HEROKU_APP_NAME_WEB`

Setup 3 tasks for api, identity, web which are Bash scripts. Each script creates login for heroku CLI (creates `~/.netrc` file with API login and key), releases Heroku app with images previously published on registry.heroku.com.

Script:
```bash
echo 'create heroku _netrc file - $(HEROKU_APP_NAME_API)'
content="
machine api.heroku.com
  login $(HEROKU_USERNAME)
  password $(HEROKU_API_KEY)
machine git.heroku.com
  login $(HEROKU_USERNAME)
  password $(HEROKU_API_KEY)
"

echo $content> ~/.netrc
cat ~/.netrc

echo 'Heroku release'
heroku container:release web -a $HEROKU_APP_NAME_API
```
