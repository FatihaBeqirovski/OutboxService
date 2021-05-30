using System;
using System.IO;
using System.Text;
using System.Text.Json;
using OutboxService.Messaging.Interfaces;

namespace OutboxService.Messaging.Strategy
{
    public static class MassTransitMessageBodyFactory
    {
        public static string Create(IMessage message, string hostName)
        {
            using (var messageDocument = JsonDocument.Parse(message.Body))
            using (var memoryStream = new MemoryStream())
            using (var jsonWriter = new Utf8JsonWriter(memoryStream))
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WriteString("messageId", Guid.NewGuid().ToString("d"));
                jsonWriter.WriteString("exchangeName", message.Exchange);
                jsonWriter.WriteString("destinationAddress",GenerateDestinationAddress(message.Exchange, hostName));

                jsonWriter.WriteStartArray("messageType");
                jsonWriter.WriteStringValue(GenerateMessageType(message.Exchange));
                jsonWriter.WriteEndArray();

                jsonWriter.WritePropertyName("message");
                messageDocument.RootElement.WriteTo(jsonWriter);

                jsonWriter.WriteEndObject();
                jsonWriter.Flush();
                
                var resultJson = Encoding.UTF8.GetString(memoryStream.ToArray());
                return resultJson;
            }
        }

        private static string GenerateMessageType(String exchangeName)
        {
            return $"urn:message:{exchangeName}";
        }

        private static string GenerateDestinationAddress(String exchangeName, String hostName)
        {
            return $"rabbitmq://{hostName}/{exchangeName}";
        }
    }
}