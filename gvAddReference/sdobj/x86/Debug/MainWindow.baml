   M S B A M L   `   `   `  ����  gvAddReference    gvAddReference.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5        $��gvMainWindow��5   W     � Title$  Add a Reference to a Page��6	   $	��359��6+   $	��525��68   
 J� Uid$ gvMainWindow��6D   .��5      � $  ��6   $	��322��6   .+�5   
   �� -   5        $��	groupBox1��5   h   $��Reference Options��6   $��97��6.   $
��Left=�6:   $��	18,12,0,0q�6U   $	��Top=�6y   $	��473��6�    � DataContext 6�   �� .��5      � .+�5      �� -   5        $��filePathBox��5   /   $��102,6,47,45q�6   $ scriptPathBox��6B   �� 5	      -   5        $��label1��5	   l   $��Script File Path:��6   $��29��64   $
��Left=�6@   $��6,4,0,0q�6[   $	��Top=�6z   $��90��6�   �� 5
      -   5        $��filePathButton��5
   `   $	��...��6   $��23��6'   $��Right=�63   $��0,6,6,0q�6O   $	��Top=�6v   $��35��6�    � ToolTip$ Browse for a file��6�    � UseLayoutRounding .  6�   	� 5      -   5        $��jsRadioButton��5   m   $��
Javascript��6   $��16��63   $
��Left=�6?   $��	14,39,0,0q�6Z   $	��Top=�6�    d� 	IsChecked$
 TrueY�6�   $ 
jsComboBox��6�   $* $Add a reference to a Javascript file��6�    	� 	GroupName$ fileType��6�   	� 5      -   5        $��cssRadioButton��5   y   $	��CSS��6   $��16��6,   $
��Left=�68   $ FalseY�6S   $��
102,39,0,0q�6e   $	��Top=�6�   $ cssComboBox��6�   $# Add a reference to a CSS file��6�   $ fileType��6�   5      5      �� 5   
   -   5        $��	groupBox2��5   e   $��Page Options��6   $	��100��6)   $
��Left=�66   $��
18,127,0,0q�6Q   $	��Top=�6v   $	��473��6�   .��5      � $	��461��6   $��76��6   .+�5      �� -	   5        $��pagePathBox��5   0   $��102,10,45,43q�6   60   �� 5      -
   5        $��label2��5   j   $��Page/Directory:��6   $��27��62   $
��Left=�6>   $��6,6,0,0q�6Y   $	��Top=�6x   $	��100��6�   �� 5      -   5        $��pageBrowseButton��5   b   $	��...��6   $��23��6'   $
��Left=�63   $��
420,10,0,0q�6N   $	��Top=�6z   $��35��6�   6�   �� 5      -   5        $��directoryCheckBox��5   }   $#��Modify All Files in Directory��6   $��16��6C   $
��Left=�6O   $��	14,48,0,0q�6j   $	��Top=�6�   $	��164��6�   5      5      �� 5   
   -   5        $��	addButton��5   J   $��Add References��6   $��23��6*   $��
144,0,0,42q�66   $��Bottom=�6[   $ addRefButton��6v   $ Add the Reference above��6�    �� 	IsDefault .  $
��Left=�6�   $��91��6�   6�   �� 5   
   -   5        $��cancelButton��5   B   $��Cancel��6   $��23��6"   $��
0,0,164,42q�6.   $��Bottom=�6V   $��Right=�6q   $��85��6�   �� 5   
   -   5        $��
statusText��5   5   $��25��6   $��81,228,90,0q�6    $�� ��6G   $	��Top=�6O   5      5      