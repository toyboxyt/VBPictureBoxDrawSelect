Public Class Form1
    Private pbselc As PictureDrawSelect
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        pbselc = New PictureDrawSelect(PictureBox1, 10, 10, 2)
        Me.Controls.Add(pbselc)
        pbselc.CreatePictbox()
    End Sub

    Private Sub ToolStripButton2_CheckStateChanged(sender As System.Object, e As System.EventArgs) Handles ToolStripButton2.CheckStateChanged
        If sender.checked = True Then
            pbselc.AddEventHandler()
        Else
            pbselc.RemoveEventHandler()
        End If
    End Sub
    Private Sub PictureBox1_MouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        Console.WriteLine("pb1 move")
    End Sub


    Private Sub PictureBox1_MouseUp(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        Console.WriteLine("pb1 up")

    End Sub

    Private Sub PictureBox1_MouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        Console.WriteLine("pb1 down")

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
    End Sub
End Class
