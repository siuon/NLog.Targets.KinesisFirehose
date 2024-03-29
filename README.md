# NLog.Targets.KinesisFirehose

This NuGet package supports AWS Kinesis Firehose, including both key authentication and AWS IAM authentication. 

If the target's AwsAccessKeyId and AwsSecretAccessKey attributes are not set, it will default to using AWS IAM authentication.

## Configuration

sample nlog.config
```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      throwExceptions="true"
      throwConfigExceptions="true"
      internalLogFile="internal-log.log"
      >

    <extensions>
        <add assembly="NLog.Targets.KinesisFirehose" />
    </extensions>
    
    <targets async="true">
        <target name="myKinesisFirehoseTarget"
                xsi:type="KinesisFirehose"
                streamName="your-stream-name"
                endPointName="end-point-name"
                >
            <field name="MyField1" layout="${longdate}" layoutType="System.DateTime" />
            <field name="MyFiled2" layout="${message}" />
        </target>
    </targets>

    <rules>
        <logger name="*" minlevel="Trace" writeTo="myKinesisFirehoseTarget" />
    </rules>
</nlog>
```

filed layotuType support below types:
- System.Boolean
- System.DateTime
- System.Double
- System.Int16
- System.Int32
- System.Int64
