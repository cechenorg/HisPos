   M S B A M L   `   `   `  ����  His_Posz    tHis_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicinesSendSingdeWindow.MedicinesSendSingdeWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        � Title$  傳送藥健康��5      $	��900��5      $	��450��5	       � WindowStartupLocation$ CenterScreen=�5
       � WindowStyle$
 None=�5      .��5      �  � RowDefinitions 5   
   �� 5      �� 5      $��50 �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��MedicinesList��5      $��0��5      $��
10,10,10,0q�5         AutoGenerateColumns .     GridBackGround#��[  5         CanUserAddRows .    DataGridCellStyle   	CellStyle# [ 5        DataGridColumnHeaderStyle   ColumnHeaderStyle# [ 5         GridLinesVisibility$ 
Horizontal=�5      ��5      �� *PrescriptionSendData+	 �� Mode$	 TwoWay=�
 �� UpdateSourceTrigger$
 PropertyChanged=� � 	Resources 5       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%0  '��       7  7  �� 5      " ��6   .�5      ��  �@J�SnapsToDevicePixels . 6<   �� 5        �@�OverridesDefaultStyle . 6>   �� 5!       � ��$	 100��6.    5"      P� 5#       �@��Orientation$ 
Horizontal=�69   . �5$      ��  � ��$
 Auto��66   �� 5%       � ��$ 15��67   �� 5&       � ��8   69   5'      P� 5(       �@��Orientation$ Vertical=�69   . �5)      ��  � ��$ 15��66   �� 5*       � ��$
 Auto��67   �� 5+       � ��8  69   5,      5.      5/         RowStyle 50      �� 51      )  #System.Windows.Controls.DataGridRow"  6    52      P� 53       �@ 
IsSelected . . �54      ��  � ��  GridDarkerBackGround# [ 6;   55      .�57      ��  � ��
 ���� 63   �� 58       � ��
 ���� 64   �� 59       � ��$ 16(�61   �� 5:       � ��$ 40��6/   5;         Columns 5=      0  *System.Windows.Controls.DataGridTextColumn  5>      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 175 5?         
FontFamily$ Segoe UI Semibold)�5A         FontSize$ 14Q�5B         
FontWeight$
 Bold"�5C         Header$ 藥品代碼��5D         
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 5@      �� *MedId+   ElementStyle 5F      �� 5G      " ��6    .�5H      ��  � ��$ Center=�6D   �� 5I       � ��$ Center=�6B   5J      5L        5M      $	 280 5N      $ Segoe UI Semibold)�5P      $ 14Q�5Q      $
 Bold"�5R      $ 藥品名稱��5S       .  5O      �� *	MedName+ 5U      �� 5V      " ��6    .�5W      ��  � ��$
 Left=�6D   �� 5X       � ��$ Center=�6B   5Y      5[        5\         MinWidth$	 100Q�5]      $ Segoe UI Semibold)�5_      $ 14Q�5`      $
 Bold"�5a         
Foreground
 �����$ 總可用量(含載途)��5c       .  5^      �� *CanUseAmount+ 5e      �� 5f      " ��6    .�5g      ��  � ��$ Center=�6D   �� 5h       � ��$ Center=�6B   5i      5k        5l      $ 80Q�5m      $ Segoe UI Semibold)�5o      $ 14Q�5p      $
 Bold"�5q      
 �����$ 	調劑量��5s       .  5n      �� *TreatAmount+ 5u      �� 5v      " ��6    .�5w      ��  � ��$ Center=�6D   �� 5x       � ��$ Center=�6B   5y      5{        5|      $ 80Q�5}      $ Segoe UI Semibold)�5      $ 18Q�5�      $
 Bold"�5�      
 �\\��$ 	傳送量��5�       .   5~      �� *
SendAmount+ 5�      �� 5�      " ��6    .�5�      ��  � ��$ Center=�6D   �� 5�       � ��$ Center=�6B   5�      5�        5�      $ 80Q�5�      $ Segoe UI Semibold)�5�      $ 18Q�5�      $
 Bold"�5�      
 �\\��$ 	自備量��5�       .  5�      �� *PrepareAmount+ 5�      �� 5�      " ��6    .�5�      ��  � ��$ Center=�6D   �� 5�       � ��$ Center=�6B   5�      5�      5�      �� 5�   
   $��1��5�      $��8q�5�       �� Orientation$ 
Horizontal=�5�      .+�5�      �� $	��610��6   �� 5�      $	��125��5�       
 Shadow#��[ 5�      $��0q�5�      $��取消��5�      $��Segoe UI Semibold)�5�      $��16(�5�      ��5�      �� *CancelCommand+y�5�      �� *AutoRegister+�� 5�      $	��125��5�      $��10,0,0,0q�5�      
���\\��$��0q�5�      $��送出��5�      $��Segoe UI Semibold)�5�      $��16(�5�      ��5�      �� *SubmitCommand+5�      5�      5�      5�      