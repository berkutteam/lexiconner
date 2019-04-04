# lexiconner

## Development

Use these urls:
- Api - https://localhost:5006;http://localhost:5005
- Web - https://localhost:5008;http://localhost:5007

## Spec
- Each study word, card, etc named as 'StudyItem'.
- Db is MongoDb. Used Atlas free tier with 0.5 GB storage.
- Sensitive information is stored in .env file in Development (use DotNetEnv to load from file) and in env variables in Production
- Hosted on Heriku with CircleCI

- RUNTIME_ENV stores environment where app is running (local, heroku, azure, ...) 

## Heroku
On heroku Docker EXPOSE doesn't work, so you need to use $PORT env variable to listen proper port
The web process must listen for HTTP traffic on $PORT, which is set by Heroku. EXPOSE in Dockerfile is not respected, but can be used for local testing. Only HTTP requests are supported.
