import { Kafka, Partitioners} from "kafkajs";

const kafka = new Kafka({
    clientId: 'test-consumer',
    GroupId: 'test-group',
    SecurityProtocol: 'PLAINTEXT',
    brokers: ['localhost:9092']
});
const admin = kafka.admin();
const producer = kafka.producer({createPartitioner: Partitioners.LegacyPartitioner});

//run producer
const run = async (message, topic) => {
    await admin.connect();
    const topics = await admin.listTopics();

    if (!topics.includes(topic)) {
        await admin.createTopics({
            topics: [{topic, numPartitions: 1, replicationFactor: 1}]
        });
        console.log(`Created topic: ${topic}`);
    }

    await admin.disconnect();
    await producer.connect();
    await producer.send({
        topic,
        messages: [{key: null, value: JSON.stringify(message)}]
    }).catch(error => {
        console.error('Error producing message:', error);
    });
    
    console.log('Message produced.');
    await producer.disconnect();
};

export default run;