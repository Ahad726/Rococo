var dataTable;

$(document).ready(function () {

    var url = window.location.search;
    var status;
    if (url.includes("inprocess")) {
        status = "inprocess";
    }
    else if (url.includes("pending")) {
        status = "pending";
    }
    else if (url.includes("completed")) {
        status = "completed";
    }
    else if (url.includes("rejected")) {
        status = "rejected";
    }
    else {
        status = "all";
    }

    loadDataTable("GetOrderList?status=" + status);
});

function loadDataTable(url) {

    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/" + url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Order/Details/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i> 
                                </a>
                               
                            </div>
                           `;
                },
                "width": "10%"
            }
        ]
    });
}