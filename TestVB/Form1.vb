Imports DotNetSiemensPLCToolBoxLibrary.Communication
Imports DotNetSiemensPLCToolBoxLibrary.DataTypes
Imports DotNetSiemensPLCToolBoxLibrary

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Configuration.ShowConfiguration("", False)

        'Verbindungen auflisten...
        'WPFToolboxForSiemensPLCs.LibNoDaveConnectionConfiguration.GetConfigurationNames()

        Dim myConn1 As New PLCConnection("aaaa")
        Dim myConn2 As New PLCConnection("bbbb")
        myConn1.Connect()
        myConn2.Connect()


        Dim val As New PLCTag
        val.ByteAddress = 10
        val.DatablockNumber = 2
        val.LibNoDaveDataType = TagDataType.Byte
        val.LibNoDaveDataSource = TagDataSource.Datablock
        myConn1.ReadValue(val)
        MsgBox(val.Value)





    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim myConn As New Communication.PLCConnection("sps1")
        Dim val1 As New Communication.PLCTag("P#M40.0 BYTE 10")
        val1.ParseControlValueFromString("1,2,3,4,5,6,7,8,9,0")
        myConn.WriteValue(val1)
    End Sub
End Class
