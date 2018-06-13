var oUsersTablel;
var oReportsTablel;
var isTrueEmail = true;


//Load dataTable with Users list
function LoadUsersTable(url) {
    debugger;
    console.log("coming to LoadUsersTable");
  
    oUsersTablel = $("#tblUsersLst").DataTable(
        {
            
            "ajax": {
                "url": url,
                "type": "GET",
                "datatype": "json"
            },
            "columns": [{ "data": "UserId", "visible": false }, { "data": "FullName" }, { "data": "Email" }, { "data": "Group" }, { "data": "Status" }, { "data": "LastLogin" },
                {
                    "data": "Action", "orderable": false, "render": function (data, type, row) { return '<a class="btn btn-default" onClick="EditSelectedUser(' + row.UserId + ');" >Edit</a> &nbsp; <a class="btn btn-normal" id="' + row.UserId + '" onClick="DeleteSelectedUser(' + row.UserId + ');">Delete</a>' }
                }]
        });

}


//User Edit
function EditSelectedUser(id) {
    debugger;
    if (id != undefined && id != null) {       
        $("#userView").load('/User/UserManagement/EditUser?userId=' + id);
    }
}


////While editing email check is that mailId associated with other users
$(document).on('change', '#Email', function () {
    debugger;
    var txtEmail = $('#Email').val();

    $.ajax({
        type: "POST",
        url: "/User/UserManagement/EditedEmailExist",
        data: '{email: "' + txtEmail + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            debugger;
            if (data == " ")
            {               
                isTrueEmail = true;               
            }
            else {
                isTrueEmail = false;
                //Show error msg here
                alert("This email is already exist");
            }           
        },
        failure: function (response) {
            isTrueEmail = false;
            //Show error msg here
            alert("This email is already exist");
        }
    });
});


// Enable edit form submit only if the email is valid
$(document).on('click', '#editUserSubmit', function (e) {
    console.log("editUserSubmit:" + isTrueEmail);
    
    if (!isTrueEmail) {
        e.preventDefault();
        //Show error msg here
        alert("This email is already exist. So please use unique email");
    }   
});


//User Delete
function DeleteSelectedUser(id) {

    if (id != undefined && id != null) {
        var choice = confirm('Do you really want to delete this record?');
        if (choice === true) {
            $.ajax({
                type: "POST",
                url: "/User/UserManagement/DeleteUser",
                data: '{userId: "' + id + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    if (data == "The user deleted successfully.") {
                        alert("User deleted successfully");
                        $('#tblUsersLst').DataTable().ajax.reload();
                    }
                    else {
                        alert(data);
                        //Show error message                                             
                    }
                },
                failure: function (response) {
                    alert(response);
                    //Show error message      
                }
            });
        }
    }
}


//Load Report
function LoadReports(url)
{
    //Set date range by default
    $('#fromDate').datepicker();
    $('#toDate').datepicker();

    oReportsTablel = $("#tblReport").DataTable({
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json"
        },
        "columns": [{ "data": "OrderId", "visible": false }, { "data": "QuestionId", "visible": false }, { "data": "Action", "orderable": false, "render": function (data, type, row) { return '<a class="btn btn-normal" onClick="DetailReportView(\'' + row.QuestionId + '\');"><i class="glyphicon glyphicon-plus"></i></a>' } }, { "data": "Question" }, { "data": "T3B" }, { "data": "Details" }, { "data": "Total" }]
    });
}


//Report detail view
function DetailReportView(id)
{
    var fromDt = $('#fromDate').val();
    var toDt = $('#toDate').val();

    if (fromDt == null || fromDt == '' || toDt == null || toDt == '') {
        fromDt = '01/01/2017';
        toDt = new Date().toISOString().slice(0, 10);
    }

    if (id !== "undefined" && id != null) {
        $("#reportView").load('/Report/ReportManagement/ReportChart?questionId=' + id + '&frDt=' + fromDt + '&toDt=' + toDt);
    }
}


////Load Chart with Report
//function LoadReportChart(reportType)
//{
//    $.ajax({
//        type: "Post",
//        url: "/ReportManagement/GetChartData?reportType=" + reportType,
//        dataType: "Json",
//        contentType: "application/json; charset=utf-8",
//        success: function(data)
//        {
//            debugger;
//            console.log("mydata:" + data);
           
//            if (data == null) {
//                debugger
//                console.log("mydata1:" + "coming to null");
//                $("#reportView").load('/Report/ReportManagement/ReportDisplay');
//            }
//            else {
//                debugger;
//                console.log("mydata2:" + "coming to not null");
//                var resultData = data;
//                var ctx = document.getElementById("rptChart").getContext('2d');
//                var month = [];
//                var VD = [];
//                var SD = [];
//                var SS = [];
//                var VS = [];               
//                var NA = [];

//                ctx.canvas.height = '615px';

//                console.log("mydat3:" + "coming to end");




//            }
//        },
//        error: function (err) {
//            debugger;
//            alert("err:" + err);
//        }
//    });
//}

