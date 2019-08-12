Imports DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
Imports DotNetSiemensPLCToolBoxLibrary.Communication
Imports DotNetSiemensPLCToolBoxLibrary.DataTypes
Imports DotNetSiemensPLCToolBoxLibrary
Imports DotNetSiemensPLCToolBoxLibrary.Projectfiles
Imports DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
Imports DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
Imports DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
Imports System.Windows.Forms

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
        myConn.ReadValue(val1)
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
        val4.TagDataType = TagDataType.DateTime
        Dim val5 As New Communication.PLCTag("DB5.DBD6")
        val5.TagDataType = TagDataType.CharArray
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

    Private Sub cmdStopPLC_Click(sender As System.Object, e As System.EventArgs) Handles cmdStopPLC.Click
        myConn.Connect()
        myConn.PLCStop()

    End Sub

    Private Sub cmdDiagPuffer_Click(sender As System.Object, e As System.EventArgs) Handles cmdDiagPuffer.Click
        myConn.Connect()
        Dim lst As List(Of DotNetSiemensPLCToolBoxLibrary.DataTypes.DiagnosticEntry)
        lst = myConn.PLCGetDiagnosticBuffer()
        For Each entr In lst
            MessageBox.Show(entr.ToString)
        Next

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Dim searchValue As String
        Dim db As String

        Dim tag As PLCTag
        Dim prj As Step7ProjectV5
        Dim fld As Projectfolders.Step7V5.BlocksOfflineFolder
        Dim blk As S7DataBlock

        searchValue = "SymbolDB_1000.Modul1.Temp4.Value"
        db = searchValue.Split(".")(0)
        searchValue = searchValue.Substring(db.Length + 1)
        prj = New Step7ProjectV5("C:\\Users\\Jochen Kühner\\Documents\\Step7 Projekte\\Offenau\\Offenau_.s7p", False)


        fld = prj.BlocksOfflineFolders(1)

        For Each projectBlockInfo As S7ProjectBlockInfo In fld.readPlcBlocksList()
            If Not projectBlockInfo.SymbolTabelEntry Is Nothing And projectBlockInfo.SymbolTabelEntry.Symbol = db Then
                blk = fld.GetBlock(projectBlockInfo)
            End If
        Next

        If Not blk Is Nothing Then
            For Each s7DataRow As S7DataRow In S7DataRow.GetChildrowsAsList(blk.GetArrayExpandedStructure())
                If s7DataRow.StructuredName = searchValue Then
                    tag = s7DataRow.PlcTag
                End If
            Next
        End If

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim PRJ As Projectfiles.Step5Project
        Dim fld As Step5BlocksFolder
        Dim blk As S5DataBlock

        PRJ = Projectfiles.Projects.LoadProject("D:\temp\_M13\M13@@@ST.S5D", False)
        fld = PRJ.BlocksFolder
        blk = fld.GetBlock("DB5")


    End Sub
End Class
