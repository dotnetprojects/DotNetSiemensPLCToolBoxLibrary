Imports DotNetSiemensPLCToolBoxLibrary.Communication
Imports DotNetSiemensPLCToolBoxLibrary.DataTypes
Imports DotNetSiemensPLCToolBoxLibrary

Public Class Form1
    Dim myConn As New PLCConnection("myVBExample")

  
    Private Sub cmdShowConfig_Click(sender As System.Object, e As System.EventArgs) Handles cmdShowConfig.Click
        'Configuration anzeigen
        Configuration.ShowConfiguration("myVBExample", True)
        'Config Objekt neu erzeugen (falls Config geändert wurde)
        Dim myConn As New PLCConnection("myVBExample")
    End Sub

    Private Sub cmdReadMW100_Click(sender As System.Object, e As System.EventArgs) Handles cmdReadMW100.Click
        myConn.Connect()
        Dim val1 As New Communication.PLCTag("MW100")
        val1.DataTypeStringFormat = TagDisplayDataType.Hexadecimal

        MessageBox.Show(val1.ValueAsString)
    End Sub

    Private Sub cmdWriteMW100_Click(sender As System.Object, e As System.EventArgs) Handles cmdWriteMW100.Click
        myConn.Connect()
        Dim val1 As New Communication.PLCTag("MW100")
        val1.Controlvalue = 44
        myConn.WriteValue(val1)
    End Sub

    Private Sub cmdReadMulti_Click(sender As System.Object, e As System.EventArgs) Handles cmdReadMulti.Click
        Dim lst As New List(Of PLCTag)

        Dim val1 As New Communication.PLCTag("MW100")
        Dim val2 As New Communication.PLCTag("AW100")
        Dim val3 As New Communication.PLCTag("EW100")
        Dim val4 As New Communication.PLCTag("DB3.DBW4")
        val4.LibNoDaveDataType = TagDataType.DateTime
        Dim val5 As New Communication.PLCTag("DB5.DBD6")
        val5.LibNoDaveDataType = TagDataType.CharArray
        val5.ArraySize = 20

        lst.Add(val1)
        lst.Add(val2)
        lst.Add(val3)
        lst.Add(val4)
        lst.Add(val5)

        myConn.Connect()

        myConn.ReadValues(lst)

        MessageBox.Show(val1.ValueAsString)
        MessageBox.Show(val2.ValueAsString)
        MessageBox.Show(val3.ValueAsString)
        MessageBox.Show(val4.ValueAsString)
        MessageBox.Show(val5.ValueAsString)
    End Sub
End Class
