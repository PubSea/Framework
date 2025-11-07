@echo OFF

cd 00-Framework
dotnet pack -R -o ../packages

cd ../01-Messaging.EntityFramework
dotnet pack -R -o ../packages

cd ../02-Messaging.EntityFramework.Kafka
dotnet pack -R -o ../packages

cd ../03-Messaging.EntityFramework.RabbitMq
dotnet pack -R -o ../packages

cd ../04-Mediator
dotnet pack -R -o ../packages

PAUSE