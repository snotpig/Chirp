   M S B A M L   `   `   `  ����@  <Chirp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ChirpChirp  5   	       Chirp.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:Chirp     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	    � Title$ Chirp��5	   	   $	��450��6   $
��1400��6$   $	��930��5
   	   $	��250��6   ���	#FF96A9CC.��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   6*   �� 5      $��* �6   6)   �� 5      $��30 �6   6*   .+�5   
   �  � ColumnDefinitions 5      �� 5      $	��620 �6#   61   �� 5      $��* �6#   6/   �� 5      $	��200 �6#   61   .+�5      �� -   5        $��	txtFolder��5      $��5,2,2,2q�6'   $��14(�68   �� 5      -   5        $��	btnFolder��5   %   $��1��6   $
��Left=�6N   $��30��6i   $	��0,2q�6t   .��5      �� $��
Folder.png��6   6.   5      �� 5      $��2��6    �� Orientation$ 
Horizontal=�6)   $��RightToLeft=�6B   $	��2,0q�6^   $��Right=�6k   .+�5      �� -   5        $��btnAdd��5      $��Right=�6<   $��70��6X   $	��3,2q�6c   .��6p   
Add Show6z   �� 5      -   5        $��
btnAddType��5      $��Right=�6D   $��70��6`   $	��3,2q�6k   .��6x   
Edit Types6�   �� 5       -   5        $��
btnRefresh��5       $��30��6D   $	��3 2q�6O   .��5!      �� $��refresh-icon.png��6   68   5"      5#      5$      &   System.Windows.Controls.DataGrid  5%   
   -   5        $��datagrid��5%      $��1��6#      AutoGenerateColumns .     CanUserAddRows .   � 	Resources 5&         Chirp.ColourConverter   ColourConverter��%   &	          5'      6   5(         Columns 5)      0  *System.Windows.Controls.DataGridTextColumn  5*      ,  &System.Windows.Controls.DataGridColumn	   Width5  /System.Windows.Controls.DataGridLengthConverter$		 240 6%   
   MinWidth$
 40Q�61      Header$
 Show��5,      1  +System.Windows.Controls.DataGridBoundColumn   Binding 5+      �� *
ShowName+5,        5-      $		 150 6%   $
 80Q�61   $ 
Short Name��5/       5.      �� *	ShortName+5/        50      $
 40Q�6%   $ Series��52       51      �� *Series+52        53      $
 52Q�6%   $ Episode��55       54      �� *	Episode+55      4  .System.Windows.Controls.DataGridTemplateColumn  56      $	 65 6)   $
 65Q�64   $
 Date��6B      CellTemplate 57      �� 58      .R�59   	   �� ��6   �� *Date+ �� StringFormat$ {0:dd-MM-yyyy }����6I   ��  �� 	Converter# [  6�   5:         CellEditingTemplate 5<      �� 5=      .R�5>      (  "System.Windows.Controls.DatePicker  $��0q�6G      SelectedDate 6)   �� *Date+6G   5?      5A        5B      $		 310 6%   $
 50Q�61   $ Title��5D       5C      �� *Title+5D        5E      $	 * 6%   $	
 500Q�6/      
IsReadOnly . $ 	File Name��5H       5G      �� *
FileName+5H      5J      �� 5K   
   -   5        $��btnGo��5K      $��2��6   ���	#FF96A9CC.��6V   Go6Z   � 5L   
   -	   5        $��progress��5L   #   $��2��6    � Minimum$ 0Q�63    � Maximum$	 100Q�6?   $y�	Collapsed=�6M   6f   5M      5N      