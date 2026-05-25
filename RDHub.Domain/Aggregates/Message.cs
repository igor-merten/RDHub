using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RDHub.Domain.Aggregates
{
    public class Message : AggregateRoot<Guid>
    {
        public Guid AuditoryId { get; private set; }
        public string Description { get; private set; } = null!;
        public string Type { get; private set; } = null!;
        public JsonElement Body { get; private set; }

        private Message() { }

        public static Message Create(Guid auditoryId, string description, string type, JsonElement body)
        {
            if (auditoryId == Guid.Empty) 
                throw new ArgumentException("AuditoryId inválido");
            if (string.IsNullOrWhiteSpace(description)) 
                throw new ArgumentException("Description é obrigatória");
            if (string.IsNullOrWhiteSpace(type)) 
                throw new ArgumentException("Type é obrigatório");
            if (body.ValueKind == JsonValueKind.Undefined || body.ValueKind == JsonValueKind.Null)
                throw new ArgumentException("Payloads obrigatório");

            return new Message
            {
                Id = Guid.NewGuid(),
                AuditoryId = auditoryId,
                Description = description,
                Type = type,
                Body = body
            };
        }
    }
}
