   M S B A M L   `   `   `  ����  KinectRobotics    KinectRobotics.ArmControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      S OMicrosoft.Kinect.Toolkit, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null\ XMicrosoft.Kinect.Toolkit.Controls, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null2k(http://schemas.microsoft.com/kinect/2013   5      
������ $��Segoe UI)�5      $��30(�5        � 	Resources  5         OnLoadedStoryboard��%   &	        �� 5   
   6   5       � Triggers 5      :� 5   
    :� RoutedEvent$ FrameworkElement.Loaded��6   .[�5      �� -   5         ��Name$( "OnLoadedStoryboard_BeginStoryboard��5      #��[  6J   5      .��5      � -   5         �Name$ kinectRegionGrid��5       � RowDefinitions 5   
   �� 5      �� 5      $��0* �6   .+�5   
   4  .Microsoft.Kinect.Toolkit.Controls.KinectRegion  -   5          Name$ kinectRegion��5      $��	0,0,215,0q�6/   .��5      :  4Microsoft.Kinect.Toolkit.Controls.KinectScrollViewer  -   5        $��scrollViewer��5   0   $��0��6#   $��Disabled=�6D   $��Disabled=�6m   $
��Left=�6�   $	��252��6�   ���	#FFB2A8A86�   5      �� 5   
   -   5        $��ColorImageStream��5      $��	251,0,0,0q�6(   �� 5   
   -   5        $��
ElbowAngle��5      $��Label��6"   $
��Left=�62   $��
280,10,0,0q�6M   $	��Top=�6a   $	��100��6y   $��49��6�   �� 5    
   -   5        $��
WristAngle��5       $��Label��6"   $
��Left=�62   $��
385,10,0,0q�6M   $	��Top=�6a   $��99��6y   $��50��6�   5!      5"      