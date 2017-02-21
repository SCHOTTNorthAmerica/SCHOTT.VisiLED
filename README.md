#SCHOTT.VisiLED
This library is to provide easy controls for the The VisiLED MC1500, and leverages functions in the [SCHOTT.Core](https://github.com/SCHOTTNorthAmerica/SCHOTT.Core) library. The library uses ComPorts for communications and control, and provides both Synchronous and Asynchronous methods.

This library can be installed in a new project with a NuGet package (also installs the [SCHOTT.Core](https://github.com/SCHOTTNorthAmerica/SCHOTT.Core) library). In the Package Manager Console, type:
Install-Package SCHOTT.VisiLED

The [VisiLED Control](https://github.com/SCHOTTNorthAmerica/VisiLED-Control) Application uses the Asynchronous methods for all of it's controls, and may be an easier way to see how they can be applied.</p>

## Simple ComPort
The sample code below shows how to connect to a simple ComPort and send commands.

```
private static void Main(string[] args)
{
    // will connect to the first VisiLED with an open ComPort
    var comPort = VisiLEDComPort.AutoConnectComPort();

    // or we can call out a particular port number to connect too
    //var comPort = VisiLEDComPort.AutoConnectComPort("COM1");

    // see if we were able to connect to a VisiLED com port
    if (comPort == null)
        return;

    // set channel A normal power to 100%
    comPort.SetPower(Channel.A, PowerMode.Normal, 100);

    // set channel A strobe power to 100%
    comPort.SetPower(Channel.A, PowerMode.Strobe, 100);

    // set channel A to strobe mode
    comPort.SetMode(Channel.A, Mode.Strobe);

    // rotate channel A quarter turn clockwise
    comPort.Rotate(Channel.A, RotationDirection.Clockwise, 2); 
}
```

## Threaded ComPort
For applications that will have units connecting/disconnecting on a regular basis can use the ThreadedComPort. This style of ComPort uses threading, and the threads need to be closed properly before closing your application. The sample code below shows how to set up a ClosingWorker (that manages the threads), start a CVLSThreadedComPort, and tie a method call to connection events.

```
private static ClosingWorker _closingWorker;
private static VisiLEDThreadedComPort _comPort;

private static void ConnectionUpdate(ThreadedComPortBase.ConnectionUpdateArgs connectionArgs)
{
    // only send commands if we connected to a unit
    if (!connectionArgs.IsConnected)
        return;

    // let the user know what port we connected too
    Console.WriteLine($"Connected to unit on: {_comPort.PortName}");

    // set channel A to pattern
    _comPort.SetSegmentCode(Channel.A, 0xAA);

    // set channel A normal power to 100%
    _comPort.SetPower(Channel.A, PowerMode.Normal, 100);
}

private static void Main(string[] args)
{
    // create a closing worker to manage the com port thread
    _closingWorker = new ClosingWorker();

    // will connect to the first VisiLED with an open ComPort
    _comPort = new VisiLEDThreadedComPort("ComPort Thread", _closingWorker);

    // register a function to be called on connection events
    _comPort.RegisterConnectionUpdate(MessageBroker.MessageContext.DirectToData, ConnectionUpdate);

    // wait for key press
    Console.WriteLine("Press any key to shut down threads");
    while (!Console.KeyAvailable) { }
    Console.ReadKey(true);

    // set channel A normal power to 0%
    _comPort.SetPower(Channel.A, PowerMode.Normal, 0);

    // close out the threads, writing messages to the console
    _closingWorker.WaitForThreadsToCloseConsoleOutput();

    // wait for key press to close
    Console.WriteLine("Press any key to close");
    while (!Console.KeyAvailable) { }
    Console.ReadKey(true);
}
```
