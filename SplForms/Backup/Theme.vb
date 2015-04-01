' Copyright (c) 2004, SplForms (author : Chandra Hundigam)
'All rights reserved.

'Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

'Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
'Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 
'THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
'OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
'SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
'HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
'EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


Public MustInherit Class Theme
    Private sFrmBackGroundImage As String
    Private sFrmTransperncyColor As Color


    Public Property BackGroundImage()
        Get
            Return sFrmBackGroundImage
        End Get
        Set(ByVal Value)
            sFrmBackGroundImage = Value
        End Set
    End Property

    Public Property TransperncyColor()
        Get
            Return sFrmTransperncyColor
        End Get
        Set(ByVal Value)
            sFrmTransperncyColor = Value
        End Set
    End Property


    Public MustOverride Function SetTheme(ByRef frmObj As Form) As Boolean

End Class

Public Class Theme1
    Inherits Theme

    Sub New()
        'Get the Image Location
        Dim fn As String = System.Environment.CurrentDirectory() & "\" & "Bitmap1.JPG"
        Me.BackGroundImage = fn
        Me.TransperncyColor = Color.FromArgb(192, 192, 255)

    End Sub


    Public Overrides Function SetTheme(ByRef frmObj As System.Windows.Forms.Form) As Boolean

        'set the transparency color
        frmObj.TransparencyKey = Me.TransperncyColor
        'set the form background image
        frmObj.BackgroundImage.FromFile(Me.BackGroundImage)
        'Change the control settings
        Dim cntl As Control
        For Each cntl In frmObj.Controls
            If TypeOf (cntl) Is Label Then
                cntl.ForeColor = Color.Yellow
                cntl.BackColor = Color.Blue
            ElseIf TypeOf (cntl) Is ComboBox Then
                cntl.ForeColor = Color.Blue
            Else
                cntl.ForeColor = Color.Black
            End If

        Next
    End Function


End Class


Public Class Theme2
    Inherits Theme
    Sub New()
        'Get the Image Location
        Dim fn As String = System.Environment.CurrentDirectory() & "\" & "Bitmap2.JPG"
        Me.BackGroundImage = fn
        Me.TransperncyColor = Color.FromArgb(192, 192, 255)
    End Sub


    Public Overrides Function SetTheme(ByRef frmObj As System.Windows.Forms.Form) As Boolean

        'set the transparency color
        frmObj.TransparencyKey = Me.TransperncyColor
        'set the form background image
        frmObj.BackgroundImage.FromFile(Me.BackGroundImage)
        'Change the control settings
        Dim cntl As Control
        For Each cntl In frmObj.Controls
            If TypeOf (cntl) Is Label Then
                cntl.ForeColor = Color.Green
                cntl.BackColor = Color.Gold
            ElseIf TypeOf (cntl) Is ComboBox Then
                cntl.ForeColor = Color.DarkRed
            Else
                cntl.ForeColor = Color.DarkGreen
            End If
        Next
    End Function

End Class