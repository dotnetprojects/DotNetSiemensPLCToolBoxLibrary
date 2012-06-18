<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmdReadMW100 = New System.Windows.Forms.Button()
        Me.cmdWriteMW100 = New System.Windows.Forms.Button()
        Me.cmdShowConfig = New System.Windows.Forms.Button()
        Me.cmdReadMulti = New System.Windows.Forms.Button()
        Me.cmdStopPLC = New System.Windows.Forms.Button()
        Me.cmdDiagPuffer = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmdReadMW100
        '
        Me.cmdReadMW100.Location = New System.Drawing.Point(12, 45)
        Me.cmdReadMW100.Name = "cmdReadMW100"
        Me.cmdReadMW100.Size = New System.Drawing.Size(108, 27)
        Me.cmdReadMW100.TabIndex = 0
        Me.cmdReadMW100.Text = "MW100 lesen"
        Me.cmdReadMW100.UseVisualStyleBackColor = True
        '
        'cmdWriteMW100
        '
        Me.cmdWriteMW100.Location = New System.Drawing.Point(12, 78)
        Me.cmdWriteMW100.Name = "cmdWriteMW100"
        Me.cmdWriteMW100.Size = New System.Drawing.Size(108, 31)
        Me.cmdWriteMW100.TabIndex = 1
        Me.cmdWriteMW100.Text = "MW100 schreiben"
        Me.cmdWriteMW100.UseVisualStyleBackColor = True
        '
        'cmdShowConfig
        '
        Me.cmdShowConfig.Location = New System.Drawing.Point(12, 12)
        Me.cmdShowConfig.Name = "cmdShowConfig"
        Me.cmdShowConfig.Size = New System.Drawing.Size(108, 27)
        Me.cmdShowConfig.TabIndex = 0
        Me.cmdShowConfig.Text = "Show Config"
        Me.cmdShowConfig.UseVisualStyleBackColor = True
        '
        'cmdReadMulti
        '
        Me.cmdReadMulti.Location = New System.Drawing.Point(13, 129)
        Me.cmdReadMulti.Name = "cmdReadMulti"
        Me.cmdReadMulti.Size = New System.Drawing.Size(108, 42)
        Me.cmdReadMulti.TabIndex = 0
        Me.cmdReadMulti.Text = "Mehrere Werte lesen"
        Me.cmdReadMulti.UseVisualStyleBackColor = True
        '
        'cmdStopPLC
        '
        Me.cmdStopPLC.Location = New System.Drawing.Point(13, 230)
        Me.cmdStopPLC.Name = "cmdStopPLC"
        Me.cmdStopPLC.Size = New System.Drawing.Size(108, 31)
        Me.cmdStopPLC.TabIndex = 1
        Me.cmdStopPLC.Text = "Stop SPS"
        Me.cmdStopPLC.UseVisualStyleBackColor = True
        '
        'cmdDiagPuffer
        '
        Me.cmdDiagPuffer.Location = New System.Drawing.Point(13, 267)
        Me.cmdDiagPuffer.Name = "cmdDiagPuffer"
        Me.cmdDiagPuffer.Size = New System.Drawing.Size(108, 31)
        Me.cmdDiagPuffer.TabIndex = 1
        Me.cmdDiagPuffer.Text = "Diagnosepuffer"
        Me.cmdDiagPuffer.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(232, 55)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(108, 27)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Adresse Auslesen"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(151, 29)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(189, 20)
        Me.TextBox1.TabIndex = 3
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(280, 111)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(70, 79)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(361, 414)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.cmdDiagPuffer)
        Me.Controls.Add(Me.cmdStopPLC)
        Me.Controls.Add(Me.cmdWriteMW100)
        Me.Controls.Add(Me.cmdShowConfig)
        Me.Controls.Add(Me.cmdReadMulti)
        Me.Controls.Add(Me.cmdReadMW100)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdReadMW100 As System.Windows.Forms.Button
    Friend WithEvents cmdWriteMW100 As System.Windows.Forms.Button
    Friend WithEvents cmdShowConfig As System.Windows.Forms.Button
    Friend WithEvents cmdReadMulti As System.Windows.Forms.Button
    Friend WithEvents cmdStopPLC As System.Windows.Forms.Button
    Friend WithEvents cmdDiagPuffer As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Button2 As System.Windows.Forms.Button

End Class
