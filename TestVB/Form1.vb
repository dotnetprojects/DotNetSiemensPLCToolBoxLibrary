Imports DotNetSiemensPLCToolBoxLibrary.DataTypes
Imports DotNetSiemensPLCToolBoxLibrary

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Configuration.ShowConfiguration("", False)

        'Verbindungen auflisten...
        'WPFToolboxForSiemensPLCs.LibNoDaveConnectionConfiguration.GetConfigurationNames()

        Dim myConn1 As New LibNoDaveConnection("aaaa")
        Dim myConn2 As New LibNoDaveConnection("bbbb")
        myConn1.Connect()
        myConn2.Connect()


        Dim val As New LibNoDaveValue
        val.ByteAddress = 10
        val.DatablockNumber = 2
        val.LibNoDaveDataType = TagDataType.Byte
        val.LibNoDaveDataSource = TagDataSource.Datablock
        myConn1.ReadValue(val)
        MsgBox(val.Value)





    End Sub
End Class
