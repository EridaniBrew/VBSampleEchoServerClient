Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading

Namespace ClientApp
	Partial Public Class Form1
		Inherits Form

		Private myMessage As String = ""
	   Private client As New TcpClient()
        Private serverEndPoint As New IPEndPoint(IPAddress.Parse("192.168.1.30"), 1552)
        'Private serverEndPoint As New IPEndPoint(IPAddress.Loopback, 7)

		Public Sub New()
			InitializeComponent()
			client.Connect(serverEndPoint)

		End Sub

		Private Sub RtbClientKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles rtbClient.KeyDown
			If e.KeyData <> Keys.Enter OrElse e.KeyData <> Keys.Return Then
				myMessage &= ChrW(e.KeyValue)
            Else
                SendMessage(myMessage)
                myMessage = ""
            End If
		End Sub

		Private Sub SendMessage(ByVal msg As String)
			Dim clientStream As NetworkStream = client.GetStream()

			Dim encoder As New ASCIIEncoding()
			Dim buffer() As Byte = encoder.GetBytes(msg)

            rtbClient.AppendText(Environment.NewLine & "Sending msg " & msg)
			clientStream.Write(buffer, 0, buffer.Length)
			clientStream.Flush()
            ' Receive the TcpServer.response.
            Dim sleepCount As Integer = 0
            While ((Not clientStream.DataAvailable) And (sleepCount < 2000))
                Thread.Sleep(1)              ' seems to need some time to get response
                sleepCount = sleepCount + 1
            End While

			' Buffer to store the response bytes.
			Dim data(255) As Byte

			' String to store the response ASCII representation.
			Dim responseData As String = String.Empty
            If (sleepCount >= 2000) Then
                responseData = "Response Timed Out"
            End If

            ' Read the first batch of the TcpServer response bytes.
            While (clientStream.DataAvailable)
                Dim bytes As Int32 = clientStream.Read(data, 0, data.Length)
                responseData = responseData & System.Text.Encoding.ASCII.GetString(data, 0, bytes)
            End While

            rtbClient.AppendText(Environment.NewLine & "From Server " & sleepCount.ToString & ": " & responseData)
        End Sub


	End Class
End Namespace
