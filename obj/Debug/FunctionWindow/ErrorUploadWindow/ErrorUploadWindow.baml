   M S B A M L   `   `   `  ����  His_Pos@    :His_Pos.FunctionWindow.ErrorUploadWindow.ErrorUploadWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  選擇異常代碼��5   	   $	��450��6   $	��560��6%    � WindowStartupLocation$ CenterScreen=�61    � WindowStyle$
 None=�6V   .��5      (�  (� Stretch$
 Fill=�6   .��5	   
   � $��
10,10,10,0q�6   $	��530��6#    � RowDefinitions 5
      �� 5      $	��350 �6    �� 5      $��50 �6    .+�5      &   System.Windows.Controls.DataGrid  $��0��6      AutoGenerateColumns .     CanUserAddRows .  $   HorizontalScrollBarVisibility$ Disabled=�6      CanUserSortColumns .  	   EnableRowVirtualization	 .  
   GridLinesVisibility$
 
Horizontal=�5      ��5   W   �� *IcErrorCodeList+ �� SelectedItem 5   R   �� *SelectedIcErrorCode+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� � 	Resources 5       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value   HorizontalScrollBar  VerticalScrollBar��%0  '��       7   7  �� 5      " ��6   .�5      ��  �@J�SnapsToDevicePixels . 6@   �� 5       �@�OverridesDefaultStyle . 6B   �� 5       � ��$	 100��62    5      P� 5       �@��Orientation$ 
Horizontal=�6=   . �5   "   ��  � ��$
 Auto��6:   �� 5   "    � ��$ 15��6;   �� 5   "    � ��8   6=   5      P� 5       �@��Orientation$ Vertical=�6=   . �5   "   ��  � ��$ 15��6:   �� 5   "    � ��$
 Auto��6;   �� 5   "    � ��8  6=   5       5"      5#      ��5$      �� 5%       �� ResourceKey$ GridBackGround��6%      	CellStyle 5'      �� 5(      $ DataGridCellStyle��6%      ColumnHeaderStyle 5*      �� 5+      $ DataGridColumnHeaderStyle��6%      RowStyle 5-      �� 5.      )  #System.Windows.Controls.DataGridRow"  6   .�5/      ��  � ��
 ���� 67   �� 50       � ��
 ���� 68   �� 51       � ��$ 16(�65   �� 52       � ��$ 40��63    53      P� 54       �@ 
IsSelected . . �55   "   ��  � ��  GridDarkerBackGround# [ 6?   56      58         Columns 5:      0  *System.Windows.Controls.DataGridTextColumn  5;         FontSize$ 14Q�6e      
FontWeight$
 Bold"�6s   ,  &System.Windows.Controls.DataGridColumn   Header$ 異常代碼��6�      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 6)   �� *ID+$ PropertyChanged=�   ElementStyle 5<      �� 5=      " ��6$   .�5>   "   ��  � ��$ Center=�6H   �� 5?   "    � ��$ Center=�6F   5@      5B        5C          Width5  /System.Windows.Controls.DataGridLengthConverter$	  470 6)   $ 14Q�6v   $
 Bold"�6�   $ 異常原因��6�    .  65   �� *	Content+$ PropertyChanged=� 5D      �� 5E      " ��6$   .�5F   "   ��  � ��$ Center=�6H   �� 5G   "    � ��$ Center=�6F   5H      5J      5L      �� 5M      ! �� Orientation$! 
Horizontal=�6   $��1��62   .+�5N      �� $��取消��6   $	��100��6F   
���iii�
������ $��18(�6�   $��Segoe UI Semibold)�6�   $��
320,9,10,9q�6�   ��6&   �� *CancelClick+6�   �� 5O      $��確認��6   $	��100��6G   
����iA�
������ $��18(�6�   $��Segoe UI Semibold)�6�   $	��0,9q�6�   ��6&   �� *ConfirmClick+6�   5P      5Q      5R      5S      