   M S B A M L   `   `   `  ���� @  <Chirp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ChirpChirp      Chirp.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:Chirp     -     � WindowStartupLocation$  CenterScreen=� � Title$ Chirp��$	��450��$
��1400��$	��930��$	��250�����	#FF96A9CC.���  � RowDefinitions �� $��30 ��� $��* ��� $��30 �.+��  � ColumnDefinitions �� $	��620 ��� $��* ��� $	��200 �.+��� -   $��	txtFolder��$��5,2,2,2q�$��14(��� -   $��	btnFolder��$��1��$
��Left=�$��30��$	��0,2q�.���� $��
Folder.png���� $��2�� �� Orientation$ 
Horizontal=�$��RightToLeft=�$	��2,0q�$��Right=�.+��� -   $��btnAdd��$��Right=�$��70��$	��3,2q�.��
Add Show�� -   $��
btnAddType��$��Right=�$��70��$	��3,2q�.��
Edit Types�� -   $��
btnRefresh��$��30��$	��3 2q�.���� $��refresh-icon.png��&   System.Windows.Controls.DataGrid  -   $��datagrid��$��1��   AutoGenerateColumns .     CanUserAddRows .     Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 240 	   MinWidth$	 40Q�
   Header$

 Show��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
ShowName+  $	 150 $	 80Q�$
 
Short Name�� �� *	ShortName+  $	 40Q�$
 Series�� �� *Series+  $	 52Q�$
 Episode�� �� *	Episode+4  .System.Windows.Controls.DataGridTemplateColumn  $ 65 $	 65Q�$

 Date��   CellTemplate �� .R��� ���� *Date+ �� StringFormat$ {0:dd-MM-yyyy }��   CellEditingTemplate �� .R�(  "System.Windows.Controls.DatePicker  $��0q�   SelectedDate �� *Date+  $	 310 $	 50Q�$
 Title�� �� *Title+  $ * $		 500Q�$
 	File Name�� �� *
FileName+�� -   $��btnGo��$��2�����	#FF96A9CC.��Go� -	   $��progress��$��2�� � Minimum$ 0Q� � Maximum$	 100Q�$y�	Collapsed=�