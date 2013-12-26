Attribute VB_Name = "Module1"
Private mobjOutlook As Outlook.NameSpace
Private fs, fo

Private Sub GetOutlook()
         Dim objOutlook As New Outlook.Application
         Set mobjOutlook = objOutlook.GetNamespace("MAPI")

End Sub

Sub GenXmlMail()
    Dim objFolder As Folder
    Dim myMail, outFile As String
    
    '
    'set your info here
    '
    myMail = "yiming.bao@emc.com"
    outFile = "C:\baoyiming\conversation.xml"
    
    Set fs = CreateObject("Scripting.FileSystemObject")
    Set fo = fs.CreateTextFile(outFile, True, True)
          
    If mobjOutlook Is Nothing Then
       GetOutlook
    End If
             
    For Each sFolder In mobjOutlook.Folders
       For Each conFoler In sFolder.Folders
           If conFoler = "Conversation History" Then
               fo.WriteLine ("<?xml version=""1.0""?>")
               fo.WriteLine ("<Conversations>")
               fo.WriteLine ("<Items>")
               For Each objItem In conFoler.Items
                   fo.WriteLine ("<Conversation>")
                   fo.WriteLine ("<Time>")
                   fo.WriteLine (objItem.ReceivedTime)
                   fo.WriteLine ("</Time>")
                   
                   fo.WriteLine ("<Recipients>")
                   For Each rec In objItem.Recipients
                       
                       If LCase(rec.Address) <> LCase(myMail) Then
                           fo.WriteLine ("<string>")
                           fo.WriteLine (rec.Address)
                           fo.WriteLine ("</string>")
                       End If
                   Next
                   fo.WriteLine ("</Recipients>")
                   
                   fo.WriteLine ("<Body><![CDATA[")
                   fo.WriteLine (objItem.Body)
                   fo.WriteLine ("]]></Body>")
                   
                   fo.WriteLine ("</Conversation>")
               Next
               fo.WriteLine ("</Items>")
               fo.WriteLine ("</Conversations>")
           End If
       Next
    Next
    
    fo.Close
    Set fo = Nothing
    Set fs = Nothing
    
    'MsgBox "Done!"
End Sub
