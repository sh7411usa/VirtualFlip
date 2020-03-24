Imports System.Windows.Forms

Public Class outs

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Form1.LogSav.ShowDialog = DialogResult.OK Then
            Try
                My.Computer.FileSystem.WriteAllText(Form1.LogSav.FileName, TextBox1.Text, True)
            Catch ex As Exception
                Form1.output("Unexpected Response!")
            End Try
        End If
    End Sub
End Class
