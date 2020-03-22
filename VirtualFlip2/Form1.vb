Public Class Form1

    Dim imgC As Integer = 0
    Dim x1 As Integer = 0, x2 As Integer = 0, y1 As Integer = 0, y2 As Integer = 0
    Dim MouseSTR As String = "Mouse: ", MouseStillDown As Boolean = False
    Dim LP As Integer = 0, LPb As Boolean = False
    Dim tCn As Integer = 0, tcount As Boolean = False, tmax As Integer = 5
    Friend Shared IsExalt As Boolean = False
    Dim mS As New SaveFileDialog, mO As New OpenFileDialog
    Dim status_text As String = "", status_time As Integer = 0

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

        Return output.Trim
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

    Private Sub Button5_Click(sender As Object, e As EventArgs)
        Exalt_Keypad.Show()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            PictureBox1.Image = GetScreen()
            setSizes(PictureBox1.Image)
        Catch ex As Exception
            Dim log As New Logging.Log("BackgroundWorker1")
            log.WriteException(ex)
        End Try

    End Sub

    Sub setSizes(image As Image)
        PictureBox1.Location = New System.Drawing.Point(0, 24)
        PictureBox1.Size = image.Size
        Me.Size = New System.Drawing.Size(image.Width + 15, image.Height + 146)
        '(15,146)
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        RunADBCommandNO("shell am start -a android.intent.action.MAIN -c android.intent.category.HOME")
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        RunADBCommandNO("shell input keyevent 1")
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        RunADBCommandNO("shell input keyevent 2")
    End Sub

    Sub FlipKeyPad(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing) _
        Handles LinkLabel3.LinkClicked, FlipKeypadToolStripMenuItem.Click
        Exalt_Keypad.Show()
    End Sub

    Private Sub InstallAnAppToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InstallAnAppToolStripMenuItem.Click
        With mO
            .Filter = "Android App Files (*.apk)|*.apk"
            .Multiselect = False
            .ShowHelp = False
        End With
        If mO.ShowDialog = DialogResult.OK Then
            BackgroundWorker2.RunWorkerAsync("install -g " & mO.FileName)
        End If
    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        RunADBCommand(e.Argument.ToString()) 'TODO: get output to status strip
    End Sub

    Private Sub StatusL_Click(sender As Object, e As EventArgs) Handles StatusL.Click

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Timer3.Enabled = True
        status_time += 1
        If status_time > 5 Then
            status_time = 0
            Timer3.Enabled = False
            StatusL.Text = "Status"
        End If
    End Sub

    Private Sub SaveScreenshotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveScreenshotToolStripMenuItem.Click
        If IsNothing(PictureBox1.Image) Then
            Exit Sub
        End If
        mS.Filter = "Portable Networks Graphic (*.PNG)|*.png"
        Dim sugg As String = Text.Trim + "_" + (((My.Computer.Clock.LocalTime.ToString).Replace("/", ".")).Replace(" ", "_")).Replace(":", ".")
        mS.FileName = sugg
        If mS.ShowDialog() = DialogResult.OK Then
            Dim im As New Bitmap(PictureBox1.Image)
            Try
                im.Save(mS.FileName, Imaging.ImageFormat.Png)
                output("Screenshot Saved: " & mS.FileName)
            Catch ex As Exception
                MsgBox("Could not write to file """ & mS.FileName & """!" & ControlChars.NewLine & ex.ToString,
                       MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)
            End Try

        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim log As New Logging.Log("Form1_Load")
        Me.Text = RunADBCommand("get-serialno").Trim
        Dim type As String = ""
        Try
            If ((Me.Text.Remove(5, Me.Text.Count - 5)).Trim) = "VN220" Then
                IsExalt = True
            End If
        Catch ex As Exception
            log.WriteException(ex)
        End Try
        'TODO: determine if 4G flip, currently only detects exalt if adb on usb
        setFlipKeys(IsExalt)
        If Not Me.Text = "error: no devices/emulators found" Then
            output("Connected: " + Me.Text)
        Else
            output(StatusL.Text = "- No Device -")
            Me.Text = "Device Connection Fail"
        End If
    End Sub

    Sub setFlipKeys(Optional isFlip As Boolean = False)
        LinkLabel1.Visible = isFlip
        LinkLabel2.Visible = isFlip
        LinkLabel3.Visible = isFlip
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
                output("Longpress Sent: " & x1 & " " & y1)
                RunADBCommandNO("shell input longpress " & x1 & " " & y1)
                Timer2.Enabled = False
            End If
        Else
            LP = 0
            MouseSTR = "Mouse: "
            Timer2.Enabled = False
        End If

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
                output("Tap Sent: " & x1 & " " & y1)
                RunADBCommandNO("shell input tap " & x1 & " " & y1)
            Else
                output("Swipe Sent: " & x1 & " " & y1 & " " & x2 & " " & y2)
                RunADBCommandNO("shell input swipe " & x1 & " " & y1 & " " & x2 & " " & y2)
            End If
        Else
            LPb = False
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim log As New Logging.Log("Form1.Button1_Click")
        output("Text Sent: """ & TextBox1.Text & """")
        RunADBCommandNO("shell input text '" & TextBox1.Text & "'")
        TextBox1.Text = ""
    End Sub

    Sub output(message As String, Optional large As Boolean = False)
        If Not large Then
            StatusL.Text = message
        Else
            'TODO:
        End If
        Dim log As New Logging.Log("Form1.Output")
        log.WriteEntry(message)
    End Sub

    Private Sub StatusL_TextChanged(sender As Object, e As EventArgs) Handles StatusL.TextChanged
        status_time = 0
        Timer3.Enabled = True
        status_text = StatusL.Text
    End Sub
End Class