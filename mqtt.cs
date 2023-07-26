
using System.Text;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json;

public class MQTT
{
    private static JSchema? _schema;
    public static JSchema? Schema
    {
        get
        {
            if (_schema == null)
            {
                JSchemaGenerator generator = new JSchemaGenerator();
                _schema = generator.Generate(typeof(MQTT));
            }
            return _schema;
        }
    }
    public string broker_addr = "10.42.0.1"!;
    public int broker_port = 1883!;
    public string? mqtt_user = "clientuser"!;
    public string? mqtt_password = "clientpass"!;
    public string? mqtt_publish_topic = "/client/publishtopic"!;
    public string? mqtt_subscribe_topic = "/client/subscribetopic"!;
    public string? mqtt_clientID = "aufas-client";
    public MqttClient? client;

    public string GetData()
    {
        return JsonConvert.SerializeObject(this);
    }


    public MQTT()
    {
        if (mqtt_clientID == null)
        {
            this.mqtt_clientID = Guid.NewGuid().ToString();
        }
    }
    public void MQTTConnect()
    {
        System.Console.WriteLine($" [INFO] Connecting to {broker_addr}, BrokerPORT {broker_port}, User: {mqtt_user}");
        try
        {
            client = new MqttClient(broker_addr, broker_port, false, new X509Certificate(), new X509Certificate2(), MqttSslProtocols.None);
        }
        catch (Exception e)
        {
            System.Console.WriteLine($" [ERROR] Cannot connect: {e.Message}");
        }
    }
    public void MQTT_Task()
    {
        Console.WriteLine($" [INFO] MQTT_Task started Broker: {broker_addr} User: {mqtt_user} Password: {mqtt_password}");

        try {
            client!.Connect(mqtt_clientID, mqtt_user, mqtt_password);
        } catch (Exception e) {
            Console.WriteLine($"Error connecting: {e.Message}, to broker: {broker_addr}");
            System.Environment.Exit(1);
        }
        Console.WriteLine($" [INFO] Connected: {mqtt_clientID}, to broker: {broker_addr}");
        // Subscribe MQTT topic
        client.Subscribe(new string[] { mqtt_subscribe_topic! }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        
        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
    }
    public void SendMessage(string subtopic, string message){
        string effectiveTopic = mqtt_publish_topic +"/"+ subtopic;
        System.Console.WriteLine("Sub topico: " + effectiveTopic);
        client!.Publish(effectiveTopic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
    }
    private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        sendMessage(Encoding.UTF8.GetString(e.Message));
        Console.WriteLine(Encoding.UTF8.GetString(e.Message));
    }

    public Action<string> sendMessage = (message) => { };
}