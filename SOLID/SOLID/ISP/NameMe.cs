namespace SOLID.ISP
{
    internal interface JmsMessage
    {
        Destination getJMSReplyTo();
        void setJMSReplyTo(Destination replyTo);
        Destination getJMSDestination();
        void setJMSDestination(Destination Destination);
        int getJMSDeliveryMode();
        void setJMSDeliveryMode(int deliveryMode);

        string getJMSType();
        void setJMSType(string type);
        long getJMSExpiration();
        void setJMSExpiration(long expiration);
        
        Enumeration getPropertyNames();
        
        void clearBody();
    }

    public interface IAcknowledgable
    {
        void acknowledge();
    }

    public interface IPrioritable
    {
        int getJMSPriority();
        void setJMSPriority(int priority);
    }

    public interface ITimestampable
    {
        long getJMSTimestamp();
        void setJMSTimestamp(long timestamp);
    }

    public interface IMessageId
    {
        string getJMSMessageID();
        void setJMSMessageID(string id);
    }

    public interface IMessageDelivery
    {
        bool getJMSRedelivered();
        void setJMSRedelivered(bool redelivered);
        long getJMSDeliveryTime();
        void setJMSDeliveryTime(long deliveryTime);
    }

    public interface IMessageCorrelation
    {
        byte[] getJMSCorrelationIDAsBytes();
        void setJMSCorrelationIDAsBytes(byte[] correlationID);
        void setJMSCorrelationID(string correlationID);
        string getJMSCorrelationID();
    }

    public interface IMessage
    {
        void setBooleanProperty(string name, bool value);
        void setByteProperty(string name, byte value);
        void setShortProperty(string name, short value);
        void setIntProperty(string name, int value);
        void setLongProperty(string name, long value);
        void setFloatProperty(string name, float value);
        void setDoubleProperty(string name, double value);
        void setStringProperty(string name, string value);
        void setObjectProperty(string name, object value);

        void clearProperties();
        bool propertyExists(string name);
        bool getBooleanProperty(string name);
        byte getByteProperty(string name);
        short getShortProperty(string name);
        int getIntProperty(string name);
        long getLongProperty(string name);
        float getFloatProperty(string name);
        double getDoubleProperty(string name);
        string getStringProperty(string name);
        object getObjectProperty(string name);
    }

    internal class Enumeration
    {
    }

    internal class Destination
    {
    }
}