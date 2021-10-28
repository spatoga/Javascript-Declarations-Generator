# Javascript-Declarations-Generator
Program that creates Javascript Variable Declarations for html elements with ids.

Use the Test Project.  
Enter a Path to the file (.html or .aspx) you want to scan, and the program will generate declarations for all elements that has an id.  
(note: for asp controls, ClientIDMode attribute must be set to 'Static'.)  
  
If you are using asp:checkboxlist, or asp:radiobuttonlist, this program also creates declarations for hard-coded options.
ex. 
```html
    <asp:RadioButtonList ID="RadioButtonList1" runat="server" ClientIDMode="Static">
        <asp:listitem Text="h1" />
        <asp:listitem Text="h2" />
        <asp:listitem Text="h3" />
    </asp:RadioButtonList>
```
to:
```javascript
        var IDs = {
            RadioButtonList1: {
                Table: "#RadioButtonList1",
                Options: {
                    h1: "#RadioButtonList1_0",
                    h2: "#RadioButtonList1_1",
                    h3: "#RadioButtonList1_2"
                },
            }
        };
```        
You can use the nesting feature by using '__' (two underscores) to seperate your terms.
(note: the file doesnt have to be .aspx, regular html is fine.)
ex.
```html
    <!-- User1 -->
    <asp:textbox ID="User1__FirstName" runat="server" ClientIDMode="Static" />
    <asp:textbox ID="User1__LastName" runat="server" ClientIDMode="Static" />

    <!-- User2 -->
    <asp:textbox ID="User2__FirstName" runat="server" ClientIDMode="Static" />
    <asp:textbox ID="User2__LastName" runat="server" ClientIDMode="Static" />
```
to:
```javascript
        var IDs = {
            User1: {
                FirstName: "#User1__FirstName",
                LastName: "#User1__LastName"
            },
            User2: {
                FirstName: "#User2__FirstName",
                LastName: "#User2__LastName"
            }
        };
```
