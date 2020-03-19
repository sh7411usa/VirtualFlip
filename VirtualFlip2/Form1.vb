Public Class Form1

    Dim imgC As Integer = 0
    Dim x1 As Integer = 0, x2 As Integer = 0, y1 As Integer = 0, y2 As Integer = 0
    Dim MouseSTR As String = "Mouse: ", MouseStillDown As Boolean = False
    Dim LP As Integer = 0, LPb As Boolean = False
    Dim tCn As Integer = 0, tcount As Boolean = False, tmax As String = 5

    Sub ClearUp(temp As String)
        Dim log As New Logging.Log("Form1.ClearUp")
        If My.Computer.FileSystem.FileExists(temp) Then
            Try
                My.Computer.FileSystem.DeleteFile(temp)
            Catch ex As Exception
                log.WriteException(ex)
            End Try
        End If
    End Sub

    Function GetScreen(Optional UseAlt As Boolean = False) As Image
        Dim Log As New Logging.Log("Form1.GetScreen"), lg As String = "Screencap Operation"
        Dim temp As String = "temp.png"
        If tcount Then
            ClearUp(temp)
            tCn += 1
            temp = "temp" & tCn & ".png"
            ClearUp(temp)
            If tCn > tmax Then tCn = 0
        End If

        Dim localDir As String = temp, remDir As String = "/sdcard/download/" & temp

        If Not UseAlt Then
            lg += ", Regular method"
            Log.WriteEntry(lg)
            Log.WriteEntry(RunADBCommandNO("shell screencap -p " & remDir))
            Log.WriteEntry(RunADBCommand("pull " & remDir & " " & localDir))
        Else
            lg += ", Using direct stream method"
            Log.WriteEntry(lg)
            Log.WriteEntry(RunADBCommandNO("exec-out screencap -p > " & localDir))
        End If

        Dim img As Bitmap = Nothing
        Try
            img = New Bitmap(Image.FromFile(localDir))
        Catch ex As Exception
            Log.WriteException(ex)
        End Try

        If IsNothing(img) And UseAlt = False Then
            img = GetScreen(True)
        End If

        Return img
    End Function



    Sub UpdatePicBox()
        Dim Log As New Logging.Log("Form1.UpdatePicBox")
        If Not BackgroundWorker1.IsBusy Then
            Log.WriteEntry("Calling BackgroundWorker1 to update screen")
            BackgroundWorker1.RunWorkerAsync()
        Else
            Log.WriteEntry("BackgroundWorker1 is busy!")
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim Log As New Logging.Log("Form1.Timer1_Tick")
        Log.WriteEntry("Tick! (Rate: " & Timer1.Interval & ")")
        UpdatePicBox()
    End Sub

    Function RunADBCommand(args As String, Optional Serial As String = Nothing) As String
        Dim Log As New Logging.Log("Form1.RunADBCommand")

        Dim output As String, serror As String, regular As String
        Dim prefix As String = ""
        If Not IsNothing(Serial) Then
            prefix = "-s " & Serial & " "
        End If
        Dim p As New Process
        With p
            .StartInfo.FileName = "adb.exe"
            .StartInfo.Arguments = prefix & args
            .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            .StartInfo.CreateNoWindow = True
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.RedirectStandardError = True
        End With
        Log.WriteEntry("Starting process: " & p.StartInfo.FileName & " " & p.StartInfo.Arguments)
        p.Start()


        regular = p.StandardOutput.ReadToEnd()
        serror = p.StandardError.ReadToEnd()
        Log.WriteEntry("Standard Output: " & regular.Trim())
        Log.WriteEntry("Standard Error: " & serror.Trim())

        If (serror.Trim() = "") Then
            output = regular
        Else
            output = serror
        End If

        Return output
    End Function
    Function RunADBCommandNO(args As String, Optional Serial As String = Nothing, Optional wait As Boolean = False) As String
        Dim log As New Logging.Log("Form1.RunADBCommandNO")
        Dim prefix As String = ""
        If Not IsNothing(Serial) Then
            prefix = "-s " & Serial & " "
        End If
        log.WriteEntry("Shell Execute: adb.exe " & prefix & args)
        Shell("adb " & prefix & args, AppWinStyle.Hide, wait, 5000)
        Return "Reads No Output"
    End Function

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        x2 = e.X : y2 = e.Y
        Label2.Text = MouseSTR & e.X & ", " & e.Y
    End Sub
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            PictureBox1.Image = GetScreen()
        Catch ex As Exception
            Dim log As New Logging.Log("BackgroundWorker1")
            log.WriteException(ex)
        End Try

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = RunADBCommand("get-serialno")
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Dim log As New Logging.Log("Timer2_Tick")
        If MouseStillDown Then
            LP += 1
            log.WriteEntry("Longpress Counter: Tick! (" & LP & ")")
            If Not (x1 = x2 And y1 = y2) Then
                LP = 0
                MouseSTR = "SWIPE: " & x1 & ", " & y1 & " "
                Timer2.Enabled = False
                Exit Sub
            End If
            If LP >= 20 Then
                LPb = True
                LP = 0
                MouseSTR = "LONGPRESS: "
                RunADBCommandNO("shell input longpress " & x1 & " " & y1)
                Timer2.Enabled = False
            End If
        Else
            LP = 0
            MouseSTR = "Mouse: "
            Timer2.Enabled = False
        End If

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        'set start x - y, start longpress timer
        MouseSTR = "TAP: "
        MouseStillDown = True
        x1 = e.X : y1 = e.Y
        Timer2.Enabled = True

    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        'send swipe or tap - as long as long press wasnt sent
        MouseStillDown = False
        MouseSTR = "Mouse: "
        Dim log As New Logging.Log("Form1.PictureBox1_MouseUp")
        x2 = e.X : y2 = e.Y
        If Not LPb Then
            If x1 = x2 And y1 = y2 Then
                log.WriteEntry("Send Tap: " & x1 & " " & y1)
                RunADBCommandNO("shell input tap " & x1 & " " & y1)
            Else
                log.WriteEntry("Send Swipe: " & x1 & " " & y1 & " " & x2 & " " & y2)
                RunADBCommandNO("shell input swipe " & x1 & " " & y1 & " " & x2 & " " & y2)
            End If
        Else
            LPb = False
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim log As New Logging.Log("Form1.Button1_Click")
        log.WriteEntry("Sent Text: """ & TextBox1.Text & """")
        RunADBCommandNO("shell input text '" & TextBox1.Text & "'")
        TextBox1.Text = ""
    End Sub
End Class