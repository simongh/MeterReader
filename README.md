# MeterReader

This attempts to read hourly meter readings using the Hildebrand API. The data is then saved in a Sqlite database.

More information on the API can be found at their [website](https://glowmarkt.com/#/faqs/data)

You need to set a username, password & connection string.

These are can be overridden as environment variables

```shell
config__username="<your username>"
config__password="<your password"
connectionstrings__meterreader="<sqlite connection string>"
```

A Sqlite connection string would look like:

```
data source=/data/readings.db
```

So to run this with docker we use

```shell
docker run -it \
-e config__username=<your username> \
-e 'config__password=<your password>' \
-e "connectionstrings__meterreader=data source=/data/readings.db" \
-v C:\workspace\MeterReader\data\:/data \
simonhalsey/meterreader
```

#Running

The app will create the database if it doesn't exist and the schema. Next it will look in the database for settings.

If it can't find any resource settings, it will look attempt to discover them by looking up the first virtual entity & then retrieving the resource ids for gas consumption & electricity consumption. These ids are then stored in the database for the next time.

Next it will look in the readings table for the last recorded reading. If it can't find any, it'll use the settings to determine a start date. By default this is the current UTC time minus 2 days. You can override this by setting the environment variable:

```
startDate=yyyy-MM-dd HH:mm:ss
```

Readings are retrieved using an interval. By default this is per day, but can be overrriden. Refer to the [API documentation](https://docs.glowmarkt.com/GlowmarktAPIDataRetrievalDocumentationIndividualUserForBright.pdf) to find valid values. The end date defaults to now. To override this, set the environment variable `enddate` as yyyy-MM-dd HH:mm:ss.

If you're having probelms with the responses from the API, you can view the full response in the log by changing the log level

```
logging__loglevel__default=Debug
```

#Building
It's a standard dotnet 6 app. Use `dotnet build`

To make the container run

```shell
docker build -t meterreader -f .\MeterReader\Dockerfile .
```
from the solution folder

to build the container to run in ARM64 as well, you need to enable & use buildkit

```shell
export DOCKER_BUILDKIT=1
docker buildx create --name mybuilder --use
docker buildx build \
--platform linux/arm64 \
-t meterreader \
-f .\MeterReader\Dockerfile \
. \
--load
```

The `docker buildx create` command only needs running once and allows you to build multi-arch containers. Just change the platform parameter to a comma seperated list. You can't currently run a multi-arch container however.