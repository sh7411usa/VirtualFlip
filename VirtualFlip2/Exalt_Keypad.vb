Imports System.Windows.Forms

Public Class Exalt_Keypad

    Dim MouseStillDown As Boolean = False, LP As Integer = 0, keyInt As Object = "0", LPSent As Boolean = False
    Dim m As New SaveFileDialog

    Sub SendKey(keynumber As String, Optional longPress As Boolean = False) '
        Dim log As New Logging.Log("Exalt_Keypad.SendKey")
        Dim pre As String = "shell input ", lpstring As String = ""
        If longPress Then pre += "--longpress "
        log.WriteEntry("Executing: ADB " & pre + "keyevent " + keynumber)
        Dim otp As String = ""
        If longPress Then
            otp = "Long "
        End If
        otp += ("Keypress Sent: " + keynumber.ToString)
        Form1.output(otp)
        Form1.RunADBCommandNO(pre + "keyevent " + keynumber)
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Dim log As New Logging.Log("Exalt_Keypad.Timer2_Tick")
        If MouseStillDown Then
            LP += 1
            log.WriteEntry("Longpress Counter: Tick! (" & LP & ")")
            If LP >= 20 Then
                LP = 0
                LPSent = True
                SendKey(GetKeyNum(keyInt), True)
                Timer2.Enabled = False
            End If
        Else
            LP = 0
            Timer2.Enabled = False
        End If
    End Sub

    Sub Buttons_MouseDown(sender As Object, e As MouseEventArgs) Handles _
        Button1.MouseDown,
        Button2.MouseDown,
        Button3.MouseDown,
        Button4.MouseDown,
        Button5.MouseDown,
        Button6.MouseDown,
        Button7.MouseDown,
        Button8.MouseDown,
        Button9.MouseDown,
        Button10.MouseDown,
        Button11.MouseDown,
        Button12.MouseDown,
        Button13.MouseDown,
        Button14.MouseDown,
        Button15.MouseDown,
        Button16.MouseDown,
        Button17.MouseDown,
        Button18.MouseDown,
        Button19.MouseDown,
        Button20.MouseDown,
        Button21.MouseDown,
        Button22.MouseDown,
        Button23.MouseDown,
        Button24.MouseDown,
        Button25.MouseDown,
        Button26.MouseDown,
        Button27.MouseDown,
        Button28.MouseDown,
        Button30.MouseDown

        keyInt = sender
    End Sub

    Sub Buttons_MouseUp(sender As Object, e As MouseEventArgs) Handles _
        Button1.MouseUp,
        Button2.MouseUp,
        Button3.MouseUp,
        Button4.MouseUp,
        Button5.MouseUp,
        Button6.MouseUp,
        Button7.MouseUp,
        Button8.MouseUp,
        Button9.MouseUp,
        Button10.MouseUp,
        Button11.MouseUp,
        Button12.MouseUp,
        Button13.MouseUp,
        Button14.MouseUp,
        Button15.MouseUp,
        Button16.MouseUp,
        Button17.MouseUp,
        Button18.MouseUp,
        Button19.MouseUp,
        Button20.MouseUp,
        Button21.MouseUp,
        Button22.MouseUp,
        Button23.MouseUp,
        Button24.MouseUp,
        Button25.MouseUp,
        Button26.MouseUp,
        Button27.MouseUp,
        Button28.MouseUp,
        Button30.MouseUp

        'run sendkey if not LPSent
        If Not LPSent Then
            SendKey(GetKeyNum(sender))
        End If
        LPSent = False
    End Sub

    Function GetKeyNum(sender As Object, Optional e As MouseEventArgs = Nothing) As String
        Dim log As New Logging.Log("Exalt_Keypad.GetKeyNum")
        Dim keyNumber As String = ""
        log.WriteEntry("Called by: Name=""" & sender.name & """ Text=""" & sender.text & """")
        Select Case sender.name
            Case "Button1"
                'Left Soft Key
                keyNumber = "1"
            Case "Button2"
                'Right Soft Key
                keyNumber = "2"
            Case "Button3"
                'Dpad Right
                keyNumber = "22"
            Case "Button4"
                'Dpad Center
                keyNumber = "23"
            Case "Button5"
                'Mic Key
                keyNumber = "243"
            Case "Button6"
                'Dpad Left
                keyNumber = "21"
            Case "Button7"
                'Dpad Up
                keyNumber = "19"
            Case "Button8"
                'Speaker
                keyNumber = "178"
            Case "Button9"
                'Dpad Down
                keyNumber = "20"
            Case "Button10"
                'Clear
                keyNumber = "28"
            Case "Button11"
                'TODO: Send
                keyNumber = ""
            Case "Button12"
                'End
                keyNumber = "6"
            Case "Button13"
                '3
                keyNumber = "10"
            Case "Button14"
                '1
                keyNumber = "8"
            Case "Button15"
                '2
                keyNumber = "9"
            Case "Button16"
                '9
                keyNumber = "16"
            Case "Button17"
                '7
                keyNumber = "14"
            Case "Button18"
                '8
                keyNumber = "15"
            Case "Button19"
                '6
                keyNumber = "13"
            Case "Button20"
                '4
                keyNumber = "11"
            Case "Button21"
                '5
                keyNumber = "12"
            Case "Button22"
                '#
                keyNumber = "18"
            Case "Button23"
                '*
                keyNumber = "17"
            Case "Button24"
                '0
                keyNumber = "7"
            Case "Button25"
                'Camera Key
                keyNumber = "27"
            Case "Button26"
                'Vol+
                keyNumber = "24"
            Case "Button27"
                'Vol-
                keyNumber = "25"
            Case "Button28"
                'Home
                keyNumber = "3"
            Case "Button30"
                'Power
                keyNumber = "26"
            Case Else
                keyNumber = ""
        End Select
        log.WriteEntry("Key Selected: " & keyNumber)
        Return keyNumber
    End Function

End Class
