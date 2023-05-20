﻿using InfluxDB.Client;
using InfluxDB.Client.Linq;
using System.Text;

namespace Oiski.School.THOP.Api.Services.Influx
{
    public class InfluxService
    {
        private readonly ILogger<InfluxService> _logger = null!;
        private readonly string _bucketName = null!;
        private readonly string _orgId = null!;
        private readonly string _url = null!;
        private readonly string _token = null!;

        public InfluxService(ILogger<InfluxService> logger, IConfiguration configuration)
        {
            _bucketName = configuration["Influx:BucketName"]!;
            _orgId = configuration["Influx:OrgId"]!;
            _url = configuration["Influx:Url"]!;
            _token = configuration["Influx:Token"]!;

            _logger = logger;
        }

        /// <summary>
        /// Write data into the influx database <paramref name="table"/> as a collection of <see cref="KeyValuePair{TKey, TValue}"/> <see langword="objects"/>
        /// <br/>
        /// <strong>Note:</strong> This will format everything that can't be parsed as a <see langword="double"/>, as a <see langword="string"/>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tag"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteAsync(string table, KeyValuePair<string, string> tag, params KeyValuePair<string, string>[] data)
        {
            _logger.LogInformation("Writing data to: {Bucket}", _bucketName);
            using var client = new InfluxDBClient(_url, _token);

            var writeApi = client.GetWriteApiAsync();

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                if (double.TryParse(data[i].Value, out double result))
                    builder.Append($"{data[i].Key}={data[i].Value}{(i == data.Length - 1 ? string.Empty : ",")}");
                else
                    builder.Append($"{data[i].Key}=\"{data[i].Value}\"{(i == data.Length - 1 ? string.Empty : ",")}");

            await writeApi.WriteRecordAsync($"{table},{tag.Key}={tag.Value} {builder}", InfluxDB.Client.Api.Domain.WritePrecision.Ns, _bucketName, _orgId);
            _logger.LogInformation("Data Written");
        }

        public async Task<Dictionary<string, List<string>>> ReadAsGroups()
        {
            var data = new Dictionary<string, List<string>>();
            var flux = $"from(bucket:\"{_bucketName}\") |> range (start: 0)";

            using var client = new InfluxDBClient(_url, _token);

            var fluxTables = await client.GetQueryApi()
                .QueryAsync(flux, _orgId);
            fluxTables.ForEach(fluxTable =>
                {
                    var table = fluxTable;
                    var fluxRecords = fluxTable.Records;
                    fluxRecords.ForEach(fluxRecord =>
                    {
                        if (!data.ContainsKey(fluxRecord.GetField()))
                            data.Add(fluxRecord.GetField(), new List<string>());

                        data[fluxRecord.GetField()].Add(fluxRecord.GetValue().ToString()!);
                    });
                });

            return data;
        }

        public List<TType> Read<TType>()
        {
            using var client = new InfluxDBClient(_url, _token);
            var queryApi = client.GetQueryApiSync();

            var query = from s in InfluxDBQueryable<TType>.Queryable(_bucketName, _orgId, queryApi) select s;

            return query.ToList();
        }
    }
}
