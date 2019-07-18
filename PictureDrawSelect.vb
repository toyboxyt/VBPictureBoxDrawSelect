Imports System.Windows.Forms
Imports System.Drawing

Public Class PictureDrawSelect
    Inherits PictureBox
    Private bmp As Bitmap '描画領域
    Private parentControl As Control '親コントロール PictureBoxを想定
    Private Property dbl_ShotSizeW_mm As Double = 10.0
    Private Property dbl_ShotSizeH_mm As Double = 10.0
    Private Property Zoom As Double
    Private dutSizeX As Double
    Private dutSizeY As Double
#Region "プロパティ"
    Private Property Color_Selection1 As Color = Color.GreenYellow
    Public Property MapSize As Size
        Get
            Return Me.Size
        End Get
        Set(value As Size)
            Me.Size = value
        End Set
    End Property
#End Region


    Sub New(ByRef in_parentControl As Control, ByVal in_dShotSizeW As Double, ByVal in_dShotSizeH As Double, ByVal in_Zoom As Double)
        parentControl = in_parentControl
        SetShotSize(in_dShotSizeW, in_dShotSizeH)
        SetZoom(in_Zoom)
        UpdateDutSize()
    End Sub

    Private Sub UpdateDutSize()
        dutSizeX = dbl_ShotSizeW_mm * Zoom
        dutSizeY = dbl_ShotSizeH_mm * Zoom
    End Sub

    Public Sub SetShotSize(ByVal in_dShotSizeW As Double, ByVal in_dShotSizeH As Double)
        dbl_ShotSizeH_mm = in_dShotSizeH
        dbl_ShotSizeW_mm = in_dShotSizeW
    End Sub
    Public Sub SetZoom(ByVal in_Zoom As Double)
        Zoom = in_Zoom
    End Sub

    Public Sub SetUpdateShotDutSize(ByVal in_size As Size, ByVal in_dShotSizeW As Double, ByVal in_dShotSizeH As Double, ByVal in_Zoom As Double)
        SetShotSize(in_dShotSizeW, in_dShotSizeH)
        SetZoom(in_Zoom)
        MapSize = in_size
        UpdateDutSize()
    End Sub

    Private Sub InitializeComponent()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureDrawSelect
        '
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    ''' <summary>
    ''' 作成処理 NewでするとTabIndexがおかしくなるため
    '''  Controls.Add()
    ''' されてから呼び出すこと
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CreatePictbox()
        Me.Parent = parentControl
        Me.MapSize = parentControl.Size
        Me.BackColor = Color.Transparent
        Me.Location = New Point(0, 0)
        'Me.TabIndex = parentControl.TabIndex + 1

        NewCreateDispose_bmp()

        RemoveEventHandler()
    End Sub
    Public Sub SetTabIndex(ByVal tbi As Integer)
        Me.TabIndex = tbi
    End Sub
    Private Sub NewCreateDispose_bmp()
        If Not IsNothing(bmp) Then
            bmp.Dispose()
            bmp = Nothing
        End If
        bmp = New Bitmap(Me.Size.Width, Me.Size.Height)
    End Sub

    Public Sub AddEventHandler()
        If Enabled = False Then
            Me.Enabled = True
            AddHandler Me.MouseDown, AddressOf PictureDrawSelect_MouseDown
            AddHandler Me.MouseMove, AddressOf PictureDrawSelect_MouseMove
            AddHandler Me.MouseUp, AddressOf PictureDrawSelect_MouseUp
        End If
    End Sub
    Public Sub RemoveEventHandler()
        If Enabled = True Then
            Me.Enabled = False
            RemoveHandler Me.MouseDown, AddressOf PictureDrawSelect_MouseDown
            RemoveHandler Me.MouseMove, AddressOf PictureDrawSelect_MouseMove
            RemoveHandler Me.MouseUp, AddressOf PictureDrawSelect_MouseUp
        End If
    End Sub




#Region "描画"

    Dim Select1 As New Drawing.Point(0, 0)
    Dim Select2 As New Drawing.Point(0, 0)
    Dim FSelect1 As New Drawing.PointF(0, 0)
    Dim FSelect2 As New Drawing.PointF(0, 0)

    Private Function Draw(Optional forceRedraw As Boolean = False) As Boolean
        Dim bRes As Boolean = False
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            g.Clear(Color.Transparent)

            Try

                ' 選択範囲描画

                '' 選択範囲が本当に変わった時だけ再描画（重くなるので）
                Dim x1 As Integer, y1 As Integer
                Dim x2 As Integer, y2 As Integer
                Dim x2last As Integer, y2last As Integer
                Dim fx1 As Single, fy1 As Single
                Dim fx2 As Single, fy2 As Single
                Dim fx2last As Single, fy2last As Single

                Dim kX As Double, kY As Double

                kX = dutSizeX
                kY = dutSizeY

                fx1 = RawSelectFirst.X / kX
                fy1 = RawSelectFirst.Y / kY
                fx2 = RawSelectMoveUp.X / kX
                fy2 = RawSelectMoveUp.Y / kY
                x1 = Math.Round(fx1)
                y1 = Math.Round(fy1)
                x2 = Math.Round(fx2)
                y2 = Math.Round(fy2)
                fx2last = RawSelectLast.X / kX
                fy2last = RawSelectLast.Y / kY
                x2last = Math.Round(fx2last)
                y2last = Math.Round(fy2last)

                If Not (forceRedraw) AndAlso (x2 = x2last And y2 = y2last) Then
                    Return True
                End If

                Dim pen1 As New Pen(Color_Selection1, 3)

                FSelect1.X = Math.Min(fx1, fx2) * kX
                FSelect1.Y = Math.Min(fy1, fy2) * kY
                FSelect2.X = Math.Max(fx1, fx2) * kX
                FSelect2.Y = Math.Max(fy1, fy2) * kY
                Select1.X = Math.Min(x1, x2) * kX
                Select1.Y = Math.Min(y1, y2) * kY
                Select2.X = Math.Max(x1, x2) * kX
                Select2.Y = Math.Max(y1, y2) * kY
                Dim w_ As Integer = Math.Abs(Select1.X - Select2.X)
                Dim h_ As Integer = Math.Abs(Select1.Y - Select2.Y)

                g.DrawRectangle(pen1, Select1.X, Select1.Y, w_, h_)
                'bmp_Selection1.MakeTransparent(Color_Transparent)

                Me.Image = bmp
            Catch ex As Exception

            Finally
            End Try


            bRes = True
        End Using
        Return bRes
    End Function
#End Region

#Region "描画選択範囲"
    Dim RawSelectFirst As New Drawing.Point(0, 0)
    Dim RawSelectMoveUp As New Drawing.Point(0, 0)
    Dim RawSelectLast As New Drawing.Point(0, 0)
    Dim IsDrugging As Boolean = False

    Private Sub PictureDrawSelect_MouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        RawSelectFirst = Me.PointToClient(Cursor.Position)
        RawSelectLast = RawSelectFirst
        IsDrugging = True
    End Sub

    Private Sub PictureDrawSelect_MouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        If IsDrugging Then
            RawSelectLast = RawSelectMoveUp
            RawSelectMoveUp = Me.PointToClient(Cursor.Position)
            'Console.WriteLine(String.Format("Dragging: {0}, {1}", RawSelectMoveUp.X, RawSelectMoveUp.Y))
            Draw()
        End If

    End Sub

    Private Sub PictureDrawSelect_MouseUp(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        If IsDrugging Then
            RawSelectLast = RawSelectMoveUp
            RawSelectMoveUp = Me.PointToClient(Cursor.Position)
            'Console.WriteLine(String.Format("MouseUp: {0}, {1}", RawSelectMoveUp.X, RawSelectMoveUp.Y))
            IsDrugging = False
            Draw()
        End If
    End Sub

#End Region

End Class
