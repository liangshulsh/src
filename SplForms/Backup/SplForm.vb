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

Imports System
Public Class frmSplForm
    Inherits System.Windows.Forms.Form
    Private mouseOffset As Point
    Private isMouseDown As Boolean = False
    Private oTheme As Theme
    Private selectedBit As Boolean = False


#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Add any initialization after the InitializeComponent() call
        cmbTheme.Items.Add("Theme1")
        cmbTheme.Items.Add("Theme2")



    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents butDestroy As System.Windows.Forms.Button
    Friend WithEvents butOK As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cmbTheme As System.Windows.Forms.ComboBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.butDestroy = New System.Windows.Forms.Button
        Me.butOK = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.cmbTheme = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'butDestroy
        '
        Me.butDestroy.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.butDestroy.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butDestroy.Location = New System.Drawing.Point(400, 64)
        Me.butDestroy.Name = "butDestroy"
        Me.butDestroy.Size = New System.Drawing.Size(16, 16)
        Me.butDestroy.TabIndex = 4
        Me.butDestroy.Text = "X"
        '
        'butOK
        '
        Me.butOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.butOK.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butOK.Location = New System.Drawing.Point(344, 168)
        Me.butOK.Name = "butOK"
        Me.butOK.Size = New System.Drawing.Size(40, 24)
        Me.butOK.TabIndex = 2
        Me.butOK.Text = "OK"
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Label1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.SaddleBrown
        Me.Label1.Location = New System.Drawing.Point(152, 176)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 16)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Select"
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Label3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.SaddleBrown
        Me.Label3.Location = New System.Drawing.Point(200, 72)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(136, 16)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "WinForms Theme"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbTheme
        '
        Me.cmbTheme.Location = New System.Drawing.Point(208, 168)
        Me.cmbTheme.Name = "cmbTheme"
        Me.cmbTheme.Size = New System.Drawing.Size(121, 21)
        Me.cmbTheme.TabIndex = 8
        '
        'frmSplForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(536, 352)
        Me.Controls.Add(Me.cmbTheme)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.butOK)
        Me.Controls.Add(Me.butDestroy)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSplForm"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub frmSplForm_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint

        Try

            If cmbTheme.SelectedItem = "Theme1" And selectedBit = True Then
                'Get the Image Location
                Dim imageFile As Image = Image.FromFile(oTheme.BackGroundImage)

                Dim shape As New System.Drawing.Drawing2D.GraphicsPath
                'Create the Ellipse shape
                shape.AddEllipse(0, 0, imageFile.Width, imageFile.Height)
                'Clip the Shape to the form region
                Me.Region = New System.Drawing.Region(shape)
                'Draw image to screen.
                e.Graphics.DrawImage(imageFile, New PointF(0.0F, 0.0F))

            ElseIf cmbTheme.SelectedItem = "Theme2" And selectedBit = True Then
                'Get the Image Location
                Dim imageFile As Image = Image.FromFile(oTheme.BackGroundImage)

                Dim shape As New System.Drawing.Drawing2D.GraphicsPath
                Dim rect As New Rectangle(0, 0, imageFile.Width + 100, imageFile.Height + 100)
                'Create the Arc shape
                shape.StartFigure()
                shape.AddArc(rect, 0, 280)
                shape.CloseFigure()
                'Clip the Shape to the form region
                Me.Region = New System.Drawing.Region(shape)

                'Draw image to screen.
                e.Graphics.DrawImage(imageFile, New PointF(0.0F, 0.0F))
            End If

            selectedBit = False
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub frmSplForm_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
        'Code for moving the form when dragged with mouse
        If isMouseDown Then
            Dim mousePos As Point = Control.MousePosition
            mousePos.Offset(mouseOffset.X, mouseOffset.Y)
            Me.Location = mousePos
        End If

    End Sub

    Private Sub frmSplForm_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        ' Changes the isMouseDown field so that the form does
        ' not move unless the user is pressing the left mouse button.
        If e.Button = MouseButtons.Left Then
            isMouseDown = False
        End If

    End Sub

    Private Sub frmSplForm_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        Dim xOffset As Integer
        Dim yOffset As Integer

        If e.Button = MouseButtons.Left Then
            xOffset = -e.X - SystemInformation.FrameBorderSize.Width
            yOffset = -e.Y - SystemInformation.CaptionHeight - _
                    SystemInformation.FrameBorderSize.Height
            mouseOffset = New Point(xOffset, yOffset)
            isMouseDown = True
        End If
    End Sub

    Private Sub butDestroy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butDestroy.Click
        Me.Dispose()
        Application.Exit()
    End Sub

    Private Sub butOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butOK.Click
        If cmbTheme.SelectedItem = "Theme1" Then
            oTheme = New Theme1
            oTheme.SetTheme(Me)
            selectedBit = True

        ElseIf cmbTheme.SelectedItem = "Theme2" Then
            oTheme = New Theme2
            oTheme.SetTheme(Me)
            selectedBit = True

        End If
        Me.ResizeRedraw = True
        Me.Refresh()

    End Sub

End Class
