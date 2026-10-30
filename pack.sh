#!/bin/bash
set -euo pipefail

project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

for project in \
    00-Framework \
    01-Messaging.EntityFramework \
    02-Messaging.EntityFramework.Kafka \
    03-Messaging.EntityFramework.RabbitMq \
    04-Mediator
do
    (
        cd "$project_root/$project"
        dotnet pack -R -o "$project_root/packages"
    )
done
