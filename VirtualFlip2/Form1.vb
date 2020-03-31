Imports System.Threading
Public Class Form1

    Dim imgC As Integer = 0
    Dim x1 As Integer = 0, x2 As Integer = 0, y1 As Integer = 0, y2 As Integer = 0
    Dim MouseSTR As String = "Mouse: ", MouseStillDown As Boolean = False
    Dim LP As Integer = 0, LPb As Boolean = False
    Dim tCn As Integer = 0, tcount As Boolean = False, tmax As Integer = 5
    Friend Shared IsExalt As Boolean = False
    Dim mS As New SaveFileDialog, mO As New OpenFileDialog
    Dim status_text As String = "", status_time As Integer = 0
    Dim OstClickPM As Boolean = True
    Dim logLevel As String = "D"
    Friend Shared PostBack As String = Nothing
    Dim IsAdmin As Boolean = False, adminTryCoun As Boolean = 1
    Dim scr_pasue As Boolean = False
    Dim SERIAL As String = ""


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
        If Not scr_pasue Then
            UpdatePicBox()
        End If
        enforceAdmin()
    End Sub

    Sub enforceAdmin()
        If Not IsAdmin Then
            If Not bckgrnd_wazekiller.IsBusy Then
                bckgrnd_wazekiller.RunWorkerAsync()
            End If
        Else

        End If
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

    Function RunCMDCommand(args As String) As String
        Dim Log As New Logging.Log("Form1.RunCMDCommand")

        Dim output As String = "", serror As String, regular As String
        Dim p As New Process
        With p
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = args
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
    Function RunCMDCommandNO(args As String) As String
        Dim Log As New Logging.Log("Form1.RunCMDCommandNO")

        Dim p As New Process
        With p
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.Arguments = args
            .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            '.StartInfo.CreateNoWindow = True
            .StartInfo.UseShellExecute = False
        End With
        Log.WriteEntry("Starting process: " & p.StartInfo.FileName & " " & p.StartInfo.Arguments)
        p.Start()
        Return "Returns No Output"
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
        If Not BackgroundWorker2.IsBusy Then
            With mO
                .Filter = "Android App Files (*.apk)|*.apk"
                .Multiselect = False
                .ShowHelp = False
            End With
            If mO.ShowDialog = DialogResult.OK Then
                BackgroundWorker2.RunWorkerAsync("install -g " & mO.FileName)
                output("Installing In Background...")
            End If
        Else
            output("APK Install Server is Busy...")
        End If

    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork

        Dim ou As New List(Of String), o As String = RunADBCommand(e.Argument.ToString())
        ou = o.Split(vbCrLf).ToList
        For c = 0 To ou.Count - 1
            ou(c) = Strings.Replace(Strings.Replace(Strings.Replace(ou(c).Replace(vbCrLf, ""), vbCr, ""), vbLf, ""), vbTab, "")
        Next
        PostBack = ou(ou.Count - 1)
    End Sub


    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Timer3.Enabled = True
        status_time += 1
        If status_time > 5 Then
            status_time = 0
            Timer3.Enabled = False
            Dim statusText As String = "Status"
            If IsAdmin Then
                statusText = "Administrator"
            End If
            StatusL.Text = statusText
            StatusL.ForeColor = SystemColors.ControlText
        End If
    End Sub

    Private Sub LaunchWazeToolStripMenuItem_Click(sender As Object, e As EventArgs)
        'universal solution (will prompt for waze/browser browser enabled phones)
        RunADBCommandNO("shell am start -d http://waze.com/ul")
        'apps4flip waze:    adb shell am start -n com.android.cts.waze/com.waze.FreeMapAppActivity
        'Regular waze:      adb shell am start -n com.waze/.FreeMapAppActivity
    End Sub

    Private Sub ResetJMusicStreamToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MsgBox(RunADBCommand("shell pm clear com.android.cts.jstream"))
    End Sub

    Private Sub LogcatToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogcatToolStripMenuItem.Click
        LogProvider()
    End Sub

    Sub LogProvider(Optional tag As String = Nothing, Optional level As String = Nothing, Optional dump As Boolean = False)
        'todo: Update logging:
        'also, in futuer add dialog that allows multiple filterspecs as in "add filter, tag:level"
        'currently, if you open filter log and enter space separeted filter specs followed by last MULTIPLE it will work
        'EG:
        'tag1:V tag2:D tag3:I etc:D MULTIPLE

        Dim log As New Logging.Log("Form1.LogProvider")
        Dim dstring As String = " "
        If dump Then
            dstring = " -d "
        End If
        If IsNothing(tag) Then
            tag = "*"
        End If
        If IsNothing(level) Then
            level = logLevel
        End If
        Dim cmd As String = "cmd /c adb logcat -s" + dstring + "filterspecs " + tag + ":" + level
        If Not BackgroundWorker3.IsBusy Then
            log.WriteEntry(cmd)
            BackgroundWorker3.RunWorkerAsync(cmd)

        Else
            output("Log provider busy")
        End If
    End Sub

    Private Sub BackgroundWorker3_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker3.DoWork
        RunCMDCommandNO(e.Argument)
    End Sub
    Private Sub BackgroundWorker4_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker4.DoWork
        showLongOutput(RunCMDCommand(e.Argument))
    End Sub

    Function GetUserInput(Optional Promt As String = "Enter input here:") As String
        Dim NoIn As String = Nothing
        Dim diaglle As New inpt
        diaglle.Label1.Text = Promt
        If diaglle.ShowDialog = DialogResult.OK Then
            NoIn = diaglle.TextBox1.Text
        End If
        Return NoIn
    End Function

    Sub cmdProvider(command As String)
        If Not BackgroundWorker4.IsBusy Then
            BackgroundWorker4.RunWorkerAsync("cmd /c " + command)
        Else
            output("CMD provider busy")
        End If
    End Sub

    Sub showLongOutput(message As String, Optional isError As Boolean = False)
        Dim out As New outs
        out.TextBox1.Text = message
        out.TextBox1.ScrollToCaret()
        If isError Then
            out.Text = "Error"
            out.TextBox1.ForeColor = Color.Red
        End If
        out.ShowDialog()
    End Sub

    Private Sub SaveScreenshotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveScreenshotToolStripMenuItem.Click
        If IsNothing(PictureBox1.Image) Then
            Exit Sub
        End If
        mS.Filter = "Portable Networks Graphic (*.PNG)|*.png"
        Dim sugg As String = Me.Text.Trim + "_" + (((My.Computer.Clock.LocalTime.ToString).Replace("/", ".")).Replace(" ", "_")).Replace(":", ".")
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
        Dim ftext As String = RunADBCommand("get-serialno").Trim
        IsAdmin = My.Settings.rem_admin
        If Not IsAdmin Then
            AdminToolStripMenuItem.Text = "Admin"
        Else
            AdminToolStripMenuItem.Text = "Log Out"
        End If
        setup_logsav()
        'TODO: determine if 4G flip, currently only detects exalt if adb on usb
        'TODO: determine if multiple devices connected
        'todo: if form exceeds host screen, zoom out! 
        setFlipKeys(IsExalt)
        Select Case ftext
            Case "error: no devices/emulators found"
                AlertAndCrash("There is no device connected, or the connection is faulty.")
            Case "error: more than one device/emulator"
                AlertAndCrash("VirtualFlip2 only works with one device connected at a time.")
            Case "'adb' is not recognized as an internal or external command, operable program or batch file."
                AlertAndCrash("VirtualFlip2 Requires ADB to run!")
            Case Else
                SERIAL = ftext
                Me.Text = ftext
                output("Connected: " + ftext)
                Try
                    If ((ftext.Remove(5, ftext.Count - 5)).Trim) = "VN220" Then
                        IsExalt = True
                    End If
                Catch ex As Exception
                    log.WriteException(ex)
                End Try
        End Select
    End Sub

    Sub AlertAndCrash(message As String)
        If Not MsgBox(message, MsgBoxStyle.Critical) = MsgBoxResult.Retry Then
            Me.Close()
        End If
    End Sub

    Private Sub CustomCommandToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CustomCommandToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter one line CMD string:")
        If Not IsNothing(response) Then
            cmdProvider(response)
        End If
    End Sub

    Private Sub FilteredLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilteredLogToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Filter Tag:")
        If Not IsNothing(response) Then
            LogProvider(response)
        End If
    End Sub

    Friend Shared LogSav As New SaveFileDialog

    Sub setup_logsav()
        LogSav.AddExtension = True
        LogSav.AutoUpgradeEnabled = True
        LogSav.DefaultExt = "log"
        LogSav.Filter = "Log files (*.log)|*.log"
    End Sub

    Private Sub DumpLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DumpLogToolStripMenuItem.Click
        Dim sugg As String = Text.Trim + "_" + (((My.Computer.Clock.LocalTime.ToString).Replace("/", ".")).Replace(" ", "_")).Replace(":", ".")
        LogSav.FileName = sugg
        If LogSav.ShowDialog = DialogResult.OK Then
            Try
                My.Computer.FileSystem.WriteAllText(LogSav.FileName, RunADBCommand("logcat -d -s filterspecs *:" + logLevel), True)
            Catch ex As Exception
                output("Unexpected Response!")
            End Try
        End If
    End Sub

    Private Sub DumpFilteredLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DumpFilteredLogToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Filter Tag:")
        Dim msug As String = ""
        If Not response.Contains(":") Then
            msug = response
        End If
        Dim sugg As String = Text.Trim + msug + "_" + (((My.Computer.Clock.LocalTime.ToString).Replace("/", ".")).Replace(" ", "_")).Replace(":", ".")
        LogSav.FileName = sugg
        If Not IsNothing(response) Then
            If LogSav.ShowDialog = DialogResult.OK Then
                Try
                    My.Computer.FileSystem.WriteAllText(LogSav.FileName, RunADBCommand("logcat -d -s filterspecs " + response + ":" + logLevel), True)
                Catch ex As Exception
                    output("Unexpected Response!")
                End Try
            End If
        Else
            Exit Sub
        End If

    End Sub

    Private Sub ListFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListFeaturesToolStripMenuItem.Click
        output(RunADBCommand("shell pm list features"), True)
    End Sub

    Private Sub ListUsersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListUsersToolStripMenuItem.Click
        output(RunADBCommand("shell pm list users"), True)
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem.Click
        output(RunADBCommand("shell pm list packages"), True)
    End Sub

    Private Sub AllDetailedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllDetailedToolStripMenuItem.Click
        output(RunADBCommand("shell pm list packages -f"), True)
    End Sub

    Private Sub UserOnlyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UserOnlyToolStripMenuItem.Click
        output(RunADBCommand("shell pm list packages -3"), True)
    End Sub

    Private Sub SystemOnlyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SystemOnlyToolStripMenuItem.Click
        output(RunADBCommand("shell pm list packages -s"), True)
    End Sub

    Private Sub EnabledOnlyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnabledOnlyToolStripMenuItem.Click
        output(RunADBCommand("shell pm list packages -e"), True)
    End Sub

    Private Sub DisabledOnlyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DisabledOnlyToolStripMenuItem.Click
        output(RunADBCommand("shell pm list packages -d"), True)
    End Sub

    Private Sub PermissionsGroupsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PermissionsGroupsToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permission-groups"), True)
    End Sub

    Private Sub ByGroupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByGroupToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permissions -g"), True)
    End Sub

    Private Sub PermissionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PermissionsToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permissions"), True)
    End Sub

    Private Sub AllInformationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllInformationToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permissions -f"), True)
    End Sub

    Private Sub SummaryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SummaryToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permissions -s"), True)
    End Sub

    Private Sub OnlyDangerousToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlyDangerousToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permissions -d"), True)
    End Sub

    Private Sub OnlyVisiblePermissionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlyVisiblePermissionsToolStripMenuItem.Click
        output(RunADBCommand("shell pm list permissions -u"), True)
    End Sub

    Private Sub DumpStateToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub UninstallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UninstallToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Package Name:")
        If Not IsNothing(response) Then
            output(RunADBCommand("shell pm uninstall " + response), True)
        End If
    End Sub

    Private Sub UninstallKeepInfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UninstallKeepInfoToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Package Name:")
        If Not IsNothing(response) Then
            output(RunADBCommand("shell pm uninstall -k " + response), True)
        End If
    End Sub

    Private Sub ClearDataCacheToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearDataCacheToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Package Name:")
        If Not IsNothing(response) Then
            output(RunADBCommand("shell pm clear " + response), True)
        End If
    End Sub

    Private Sub EnableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnableToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Package Name:")
        If Not IsNothing(response) Then
            output(RunADBCommand("shell pm enable " + response), True)
        End If
    End Sub

    Private Sub DisableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DisableToolStripMenuItem.Click
        Dim response As String = GetUserInput("Enter Package Name:")
        If Not IsNothing(response) Then
            output(RunADBCommand("shell pm disable " + response), True)
        End If
    End Sub

    Private Sub PauseScreenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PauseScreenToolStripMenuItem.Click
        scr_pasue = Not scr_pasue
        If Not scr_pasue Then
            PauseScreenToolStripMenuItem.Text = "Pause Screen"
        Else
            PauseScreenToolStripMenuItem.Text = "Restart Screen"
        End If
    End Sub

    Private Sub Timer_PostBack_Tick(sender As Object, e As EventArgs) Handles Timer_PostBack.Tick
        If Not IsNothing(PostBack) Then
            Dim ou As String = PostBack : PostBack = Nothing
            If ou.Contains("failure") Or ou.Contains("Failure") Or ou.Contains("FAILURE") Then
                ou = ou.Replace("failure [", "")
                ou = ou.Replace("Failure [", "")
                ou = ou.Replace("FAILURE [", "")
                ou = ou.Replace("]", "")
                ou = ou.Replace("_", " ")
                'output(ou,, True)
                output("Install: Failure",, True)
            ElseIf ou.Contains("success") Or ou.Contains("SUCCESS") Or ou.Contains("Sucess") Then
                output("Install: Success")
                StatusL.ForeColor = Color.DarkGreen
            End If
        End If
    End Sub

    Private Sub LogActivityManagerIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogActivityManagerIToolStripMenuItem.Click
        LogProvider("ActivityManager", "I")
    End Sub

    Private Sub Timer_AdminEnforcer_Tick(sender As Object, e As EventArgs)
        enforceAdmin()
    End Sub

    Private Sub AdminToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AdminToolStripMenuItem.Click
        AdministratorLogin()
    End Sub

    Private Sub bckgrnd_wazekiller_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bckgrnd_wazekiller.DoWork
        RunADBCommandNO("shell am force-stop com.android.cts.waze")
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Sub AdministratorLogin()
        Dim log As New Logging.Log("Form1.AdministratorLogin")
        If IsAdmin Then
            IsAdmin = False
            My.Settings.rem_admin = False
            My.Settings.Save()
            AdminToolStripMenuItem.Text = "Admin"
            output("Logged Out")
            Exit Sub
        End If
        Dim response As String = GetUserInput("Please Input Admin Passcode")
        If Not IsNothing(response) Then
            Try
                If response = My.Settings.pass Then
                    IsAdmin = True
                    log.WriteEntry("Logged In as Administrator")
                    output("Logged In as Administrator")
                    UserOnlyToolStripMenuItem.Visible = True
                    AdminToolStripMenuItem.Text = "Log Out"
                    If MsgBox("Stay Logged In for next session?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        My.Settings.rem_admin = True
                        My.Settings.Save()
                    End If
                    Exit Sub
                Else
                    adminTryCoun += 1
                    output(adminTryCoun + " out of 3 attempts.")
                End If
            Catch ex As Exception
                log.WriteException(ex)
                log.WriteEntry("You should only get an exception if you rebuilt the program yourself from github. If thats the case, then figure it out yourself.")
            End Try
        End If
        If adminTryCoun > 2 Then
            Me.Close()
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
        'TODO: ^escape char is not working!
        If TextBox1.Text.Contains("^") Then
            TextBox1.Text = TextBox1.Text.Replace("^", "^^")
        End If
        If TextBox1.Text.Contains("'") Then
            TextBox1.Text = TextBox1.Text.Replace("'", "^'")
        End If
        If TextBox1.Text.Contains("""") Then
            TextBox1.Text = TextBox1.Text.Replace("""", "^""")
        End If
        If TextBox1.Text.Contains("&") Then
            TextBox1.Text = TextBox1.Text.Replace("&", "^&")
        End If
        If TextBox1.Text.Contains(">") Then
            TextBox1.Text = TextBox1.Text.Replace(">", "^>")
        End If
        If TextBox1.Text.Contains("<") Then
            TextBox1.Text = TextBox1.Text.Replace("<", "^<")
        End If
        If TextBox1.Text.Contains("|") Then
            TextBox1.Text = TextBox1.Text.Replace("|", "^|")
        End If
        output("Text Sent: """ & TextBox1.Text & """")
        RunADBCommandNO("shell input text '" & TextBox1.Text & "'")
        TextBox1.Text = ""
        TextBox1.Select()
    End Sub

    Sub output(message As String, Optional large As Boolean = False, Optional IsError As Boolean = False)
        If Not large Then
            StatusL.Text = message
            If IsError Then
                StatusL.ForeColor = Color.Red
            Else
                StatusL.ForeColor = SystemColors.ControlText
            End If
        Else
            showLongOutput(message, IsError)
        End If
        Dim log As New Logging.Log("Form1.Output")
        log.WriteEntry(message)
    End Sub

    Private Sub StatusL_TextChanged(sender As Object, e As EventArgs) Handles StatusL.TextChanged
        status_time = 0
        Timer3.Enabled = True
        status_text = StatusL.Text
    End Sub

    Private Sub LogLevel_Click(sender As Object, e As EventArgs) Handles _
        DebugToolStripMenuItem.Click,
        VerboseToolStripMenuItem.Click,
        InformationToolStripMenuItem.Click,
        WarningsToolStripMenuItem.Click,
        ErrorToolStripMenuItem.Click,
        FatalToolStripMenuItem.Click,
        SupressToolStripMenuItem.Click

        DebugToolStripMenuItem.Checked = False
        VerboseToolStripMenuItem.Checked = False
        InformationToolStripMenuItem.Checked = False
        WarningsToolStripMenuItem.Checked = False
        ErrorToolStripMenuItem.Checked = False
        FatalToolStripMenuItem.Checked = False
        SupressToolStripMenuItem.Checked = False

        If sender.name = "DebugToolStripMenuItem" Then
            DebugToolStripMenuItem.Checked = True
        End If
        If sender.name = "VerboseToolStripMenuItem" Then
            VerboseToolStripMenuItem.Checked = True
        End If
        If sender.name = "InformationToolStripMenuItem" Then
            InformationToolStripMenuItem.Checked = True
        End If
        If sender.name = "WarningsToolStripMenuItem" Then
            WarningsToolStripMenuItem.Checked = True
        End If
        If sender.name = "ErrorToolStripMenuItem" Then
            ErrorToolStripMenuItem.Checked = True
        End If
        If sender.name = "FatalToolStripMenuItem" Then
            FatalToolStripMenuItem.Checked = True
        End If
        If sender.name = "SupressToolStripMenuItem" Then
            SupressToolStripMenuItem.Checked = True
        End If

        SetLogLevel()

    End Sub

    Sub SetLogLevel(Optional level As String = Nothing)
        If Not IsNothing(level) Then
            logLevel = level
        Else
            If VerboseToolStripMenuItem.Checked Then
                logLevel = "V"
            End If
            If DebugToolStripMenuItem.Checked Then
                logLevel = "D"
            End If
            If InformationToolStripMenuItem.Checked Then
                logLevel = "I"
            End If
            If WarningsToolStripMenuItem.Checked Then
                logLevel = "W"
            End If
            If ErrorToolStripMenuItem.Checked Then
                logLevel = "E"
            End If
            If FatalToolStripMenuItem.Checked Then
                logLevel = "F"
            End If
            If SupressToolStripMenuItem.Checked Then
                logLevel = "S"
            End If
        End If
        output("Log Level Set To: " & logLevel)
    End Sub
End Class