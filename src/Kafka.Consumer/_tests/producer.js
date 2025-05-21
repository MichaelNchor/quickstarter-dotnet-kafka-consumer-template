const { Kafka, Partitioners} = require('kafkajs');

const kafka = new Kafka({
    clientId: 'test-consumer',
    GroupId: 'test-group',
    SecurityProtocol: 'PLAINTEXT',
    brokers: ['localhost:9092']
});

const admin = kafka.admin();
const producer = kafka.producer({createPartitioner: Partitioners.LegacyPartitioner});

const topic = 'test-topic';

(async () => {
    await admin.connect();
    const topics = await admin.listTopics();

    if (!topics.includes(topic)) {
        await admin.createTopics({
            topics: [{ topic, numPartitions: 1, replicationFactor: 1 }]
        });
        console.log(`Created topic: ${topic}`);
    }

    await admin.disconnect();

    await producer.connect();
    const message = {
        "Id": "c19f4a0c97fe4c508bbfae221be5e45c",
        "Message": "Order successfully created",
        "OrderId": "ORD-1001",
        "CreatedAt": "2025-05-17T12:00:00Z",
        "UpdatedAt": "0001-01-01T00:00:00Z"
    };

    await producer.send({
        topic,
        messages: [{ key: null, value: JSON.stringify(message) }]
    }).catch(error => {
        console.error('Error producing message:', error);
    });

    console.log('Message produced.');
    await producer.disconnect();
})();
