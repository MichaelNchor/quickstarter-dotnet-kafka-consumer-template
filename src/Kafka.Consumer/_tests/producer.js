import run from "./Core/main.js";

const message = {
    "Id": "c19f4a0c97fe4c508bbfae221be5e45c",
    "Message": "Order successfully created",
    "OrderId": "ORD-1001",
    "CreatedAt": "2025-05-17T12:00:00Z",
    "UpdatedAt": "0001-01-01T00:00:00Z"
};

const topic = "test-topic";

run(message, topic);


