using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using System.Timers;

namespace projektSmPC
{
  public partial class MainWindow : Window
  {
    private SerialPort Port = new SerialPort();
    private System.Timers.Timer @Timer = new System.Timers.Timer(1000);
    private bool ShouldRequestMeasurments = false;

    public MainWindow()
    {
      InitializeComponent();

      Port.DataReceived += OnDataRecieved;
      button.Click += OnSendClicked;
      Open.Click += OnOpenClicked;
      Close.Click += OnCloseClicked;
      RequestMeasurmentsButton.Click += OnRequestMeasurmentsClicked;
      StopMeasurmentsButton.Click += OnStopMeasurmentsClicked;
      ResetScaleButton.Click += OnResetScaleClicked;

      Port.BaudRate = 9600;
      Port.StopBits = StopBits.One;
      Port.Parity = Parity.None;
      Port.ReadTimeout = 500;
      Port.WriteTimeout = 500;
      Close.IsEnabled = false;

      StopMeasurmentsButton.IsEnabled = false;
      RequestMeasurmentsButton.IsEnabled = false;

      Timer.Elapsed += RequestMeasurments;
      Timer.AutoReset = true;
      Timer.Enabled = true;
    }
    
    public void RequestMeasurments(Object source, ElapsedEventArgs e)
    {
      if (ShouldRequestMeasurments)
        SendMessage("a"); 
    }

    public void OnStopMeasurmentsClicked(object sender, RoutedEventArgs e)
    {
      ShouldRequestMeasurments = false;
      RequestMeasurmentsButton.IsEnabled = true;
      StopMeasurmentsButton.IsEnabled = false;
    }

    public void OnRequestMeasurmentsClicked(object sender, RoutedEventArgs e)
    { 
      ShouldRequestMeasurments = true;
      RequestMeasurmentsButton.IsEnabled = false;
      StopMeasurmentsButton.IsEnabled = true;
    }

    public void OnResetScaleClicked(object sender, RoutedEventArgs e)
    {
      SendMessage("b");
    }

    public void OnOpenClicked(object sender, RoutedEventArgs e)
    {
      try
      {
        Timer.Enabled = true;
        Port.PortName = comPortName.Text;
        Port.Open();
        Open.IsEnabled = false;
        Close.IsEnabled = true;
        RequestMeasurmentsButton.IsEnabled = true;
        StopMeasurmentsButton.IsEnabled = false;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error: " + ex.Message);
      }
    }
    
    public void OnCloseClicked(object sender, RoutedEventArgs e)
    {
      Port.Close();
      Timer.Enabled = false;
      Open.IsEnabled = true;
      Close.IsEnabled = false;
      ShouldRequestMeasurments = false;
      RequestMeasurmentsButton.IsEnabled = false;
      StopMeasurmentsButton.IsEnabled = false;
    }

    public void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
    {
      try
      {
        SerialPort temp = (SerialPort)sender;
        string xd = temp.ReadLine();
        Dispatcher.BeginInvoke(
          new ThreadStart(() => Measurment.Text = xd));
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    } 

    public void SendMessage(string s)
    {
      try
      {

        byte[] bytes = Encoding.UTF8.GetBytes(s);
        Port.Write(bytes, 0, bytes.Count());
        Timer.Start();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error: " + ex.Message);
      }
    }

    public void OnSendClicked(object sender, RoutedEventArgs e)
    {
      try
      {
        Timer.Stop();
        if (comPortName.Text != "Type COM port name here!" && Port.PortName != comPortName.Text)
          Port.PortName = comPortName.Text;

        SendMessage(typedMessage.Text);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error: " + ex.Message);
      }
    }
  }
}
