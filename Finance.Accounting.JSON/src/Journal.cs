using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.Accounting.JSON;

public class JournalConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        return typeToConvert.GetGenericTypeDefinition() == typeof(Journal<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var timeType = typeToConvert.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(JournalConverter<>).MakeGenericType(new[] { timeType }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null
        )!;

        return converter;
    }

    private class JournalConverter<Time> : JsonConverter<Journal<Time>>
        where Time : notnull, IComparable<Time>
    {
        private readonly JsonConverter<Time> _timeConverter;
        private readonly JsonConverter<List<Transaction>> _transactionsConverter;

        public JournalConverter(JsonSerializerOptions options)
        {
            _timeConverter = (JsonConverter<Time>)options.GetConverter(typeof(Time));
            _transactionsConverter = (JsonConverter<List<Transaction>>)options.GetConverter(typeof(List<Transaction>));
        }

        public override Journal<Time>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.AssertStartObject();

            var journal = new Journal<Time>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var time = _timeConverter.ReadAsPropertyName(ref reader, typeof(Time), options);
                reader.Read();
                var transactions = _transactionsConverter.Read(ref reader, typeof(List<Transaction>), options);

                journal[time] = transactions;
            }

            reader.AssertEndObject();

            return journal;
        }

        public override void Write(Utf8JsonWriter writer, Journal<Time> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var point in value.OrderBy(point => point.Time))
            {
                _timeConverter.WriteAsPropertyName(writer, point.Time, options);
                _transactionsConverter.Write(writer, point.Event, options);
            }

            writer.WriteEndObject();
        }
    }
}