# What is OCH?  
OCH is a program to record communicator conversation history.  
   
# Why OCH?   
By default conversation history could only be saved in Outlook, however the history in Outlook will be cleaned after three months.   
   
#How to use OCH?   
Simply double-click OCH.exe, then OCH runs as a system tray at the bottom right corner of your screen. Not extra actions needed. OCH takes care of OC history automatically.   
You could open OCH GUI by clicking the tray icon. By default OCH will list all the contacts. You could select one contact, then the date list will show the recent 100 days of this contact.   
![home_1](https://cloud.githubusercontent.com/assets/5849364/25512065/b760c498-2bfd-11e7-82d7-1d0c08817d68.PNG)   
Figure 1. Default view   
   
You can also search conversations by day and keyword. OCH will highlight the keyword and scroll to the first match.   
![home_2](https://cloud.githubusercontent.com/assets/5849364/25512064/b76046da-2bfd-11e7-953f-dde98c4f591b.png)   
Figure 2. Search view   
NOTE: you can also trigger search function by Ctrl+F after conversation history area gets focus.   
  
  
  
# For users who use Outlook to save conversation history   
You could use the tools in OCH folder to migrate conversation from Outlook to OCH. Follow these steps:   
1.  Enable develop mode in Outlook:   
#### Outlook 2007   
* Open a message or create a new one.   
* Click on the Office logo in the left top and choose Editor Options.   
* In the Popular section, enable the option: Show Developer tab in the Ribbon.   
* Press OK to close the open dialog.   
   
#### Outlook 2010   
* Open a message or create a new one.   
* Click on the Office logo in the left top and choose Editor Options.   
* In the Popular section, enable the option: Show Developer tab in the Ribbon.   
* Press OK to close the open dialog.   
![home_5](https://cloud.githubusercontent.com/assets/5849364/25512069/b769d196-2bfd-11e7-8f4c-4331836c2d6b.png)   
   
2. Click Visual Basic -> File -> Import File -> select the GenConversationToXml.bas in OCH folder -> open Module1 -> replace your information in the red rectangle.   
**If macro is disabled you should enable it via Macro Security. Also it is recommended to disable macro after exporting conversation history.**   
![home_3](https://cloud.githubusercontent.com/assets/5849364/25512067/b767310c-2bfd-11e7-8557-101c4a103ea2.png)   

3. Click "Run" , serveral seconds later you will get the xml file.   
4. Open cmd. Drag ConversationMigration.exe in OCH folder onto cmd and specify the xml file generated in step 3.   
![home_4](https://cloud.githubusercontent.com/assets/5849364/25512066/b766cc94-2bfd-11e7-941d-43f3fbbdd3f9.png)   
Several seconds later you should see:   
![home_6](https://cloud.githubusercontent.com/assets/5849364/25512068/b7696c4c-2bfd-11e7-90c6-4ba636fad78d.png)   
Congrats, outlook conversations history is migrated to OCH successfully.   
   
# Known issues:
1. OCH is able to auto-start when log on to Windows.  Right click tray icon you could see the option. **BUT** because of UAC you need **Run as Administrator** to set auto-start. Also you cannot see whether auto-start is enabled or not in non-administrator role.   
2. Sometimes conversation does not show up. Workaround: just click the date again.   

# System requirement:   
.NET Framework 3.5+   
Microsoft Office Communicator 2007 R2 Version 3.5.6907.268    

# Thanks   
Thanks to Office Communicator SDK Wrapper   
Please let me know if you have good suggestions. Cheers!   
