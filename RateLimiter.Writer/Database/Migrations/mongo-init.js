db = db.getSiblingDB("writerdb");

db.createCollection("rate_limits", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["route", "requests_per_minute"],
            properties: {
                route: {
                    bsonType: "string",
                    description: "route must be a string and is required"
                },
                requests_per_minute: {
                    bsonType: "int",
                    description: "requests_per_minute must be an integer and is required"
                }
            }
        }
    }
});

db.rate_limits.createIndex({"route": 1}, { "unique": true});
