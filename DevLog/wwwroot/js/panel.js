var Panel = (function () {

    //===================================
    //          Data Tables Buttons
    //===================================
    function renderButtons(editUrl, detailsUrl, deleteUrl, id) {

        var result = "";

        if (editUrl !== undefined) {
            var linkEdit = `<a href="${editUrl}/-1" class="btn btn-primary" title="Edit"><i class="fa fa-pen"></i></a>`;
            linkEdit = linkEdit.replace("-1", id);

            result += linkEdit;
        }

        if (detailsUrl !== undefined) {
            var linkDetails = `<a href="${detailsUrl}/-1" class="btn btn-warning" title="Details"><i class="fa fa-list"></i></a>`;
            linkDetails = linkDetails.replace("-1", id);

            result += " | " + linkDetails;
        }

        if (deleteUrl !== undefined) {
            var linkDelete = `<a href="${deleteUrl}/-1" class="btn btn-danger" title="Delete"><i class="fa fa-trash"></i></a>`;
            linkDelete = linkDelete.replace("-1", id);

            result += " | " + linkDelete;
        }

        return result;
    }

    //===================================
    //    Data Tables bool to string
    //===================================
    function boolToText(val) {
        if (val === true) {
            return "Yes";
        }

        return "No";
    }

    //===================================
    // Data Tables reduce string length
    //===================================
    function reduceTextToSmallerWidth(text, charsCount) {
        return (text.length > charsCount) ? text.substr(0, charsCount - 1) + '&hellip;' : text;
    }

    //===================================
    //          Certificates table
    //===================================
    var loadCertificatesTable = function certificates(apiUrl, editUrl, detailsUrl, deleteUrl) {
        $("#jq-table").DataTable({
            responsive: true,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: true },
            // Ajax Filter
            ajax: {
                url: apiUrl,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: [
                { data: "title" },
                {
                    data: "createDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.createDate);
                    }
                },
                {
                    data: "lastEditDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.lastEditDate);
                    }
                },
                {
                    mRender: function (data, type, row) {
                        return renderButtons(editUrl, detailsUrl, deleteUrl, row.id);
                    }
                }
            ],
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: false },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }

                        return data;
                    }
                },
                { targets: "date-type", type: "date-eu" }
            ]
        });
    }

    //===================================
    //          Contacts table
    //===================================
    var loadContactsTable = function contacts(apiUrl, editUrl, detailsUrl, deleteUrl) {
        $("#jq-table").DataTable({
            responsive: true,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: true },
            // Ajax Filter
            ajax: {
                url: apiUrl,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: [
                { data: "userFullName" },
                { data: "subject" },
                { data: "emailOrPhoneNumber" },
                {
                    data: "body",
                    render: function (data, type, row) {
                        return reduceTextToSmallerWidth(row.body, 50);
                    }
                },
                { data: "ip" },
                {
                    data: "createDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.createDate);
                    }
                },
                {
                    data: "lastEditDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.lastEditDate);
                    }
                },
                {
                    mRender: function (data, type, row) {
                        return renderButtons(editUrl, detailsUrl, deleteUrl, row.id);
                    }
                }
            ],
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: false },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }

                        return data;
                    }
                },
                { targets: "date-type", type: "date-eu" }
            ]
        });
    }

    //===================================
    //            Users table
    //===================================
    var loadUsersTable = function users(apiUrl, editUrl, detailsUrl) {
        let table = $("#jq-table").DataTable({
            responsive: true,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: true },
            // Ajax Filter
            ajax: {
                url: apiUrl,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: [
                { data: "userName" },
                { data: "userFullName" },
                { data: "email" },
                {
                    data: "isActive",
                    render: function (data, type, row) {
                        return `<input type="checkbox" class="chk-user-active" id="user-${row.id}" ${row.isActive ? 'checked="true"' : ""}>`;
                    }
                },
                {
                    data: "createDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.createDate);
                    }
                },
                {
                    data: "lastEditDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.lastEditDate);
                    }
                },
                {
                    mRender: function (data, type, row) {
                        return renderButtons(editUrl, detailsUrl, undefined, row.id);
                    }
                }
            ],
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: false },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }

                        return data;
                    }
                },
                { targets: "date-type", type: "date-eu" }
            ]
        });

        return table;
    }

    //===================================
    //            Posts table
    //===================================
    var loadPostsTable = function posts(apiUrl, editUrl, detailsUrl, deleteUrl) {
        $("#jq-table").DataTable({
            responsive: true,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: true },
            // Ajax Filter
            ajax: {
                url: apiUrl,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: [
                { data: "title" },
                {
                    data: "postCategoryTitle",
                    render: function (data, type, row) {
                        if (row.postCategoryTitle === null) {
                            return "None";
                        }
                        return row.postCategoryTitle;
                    }
                },
                { data: "tags" },
                { data: "userFullName" },
                {
                    data: "isCommentsOn",
                    render: function (data, type, row) {
                        return boolToText(row.isCommentsOn);
                    }
                },
                {
                    data: "createDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.createDate);
                    }
                },
                {
                    data: "lastEditDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.lastEditDate);
                    }
                },
                {
                    mRender: function (data, type, row) {
                        return renderButtons(editUrl, detailsUrl, deleteUrl, row.id);
                    }
                }
            ],
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: false },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }

                        return data;
                    }
                },
                { targets: "date-type", type: "date-eu" }
            ]
        });
    }

    //===================================
    //      Post categories table
    //===================================
    var loadPostCategoriesTable = function cats(apiUrl, editUrl, detailsUrl, deleteUrl) {
        $("#jq-table").DataTable({
            responsive: true,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: true },
            // Ajax Filter
            ajax: {
                url: apiUrl,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: [
                { data: "title" },
                {
                    data: "createDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.createDate);
                    }
                },
                {
                    data: "lastEditDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.lastEditDate);
                    }
                },
                {
                    mRender: function (data, type, row) {
                        return renderButtons(editUrl, detailsUrl, deleteUrl, row.id);
                    }
                }
            ],
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: false },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }

                        return data;
                    }
                },
                { targets: "date-type", type: "date-eu" }
            ]
        });
    }

    //===================================
    //      Post comments table
    //===================================
    var loadPostCommentsTable = function postComments(apiUrl, editUrl, detailsUrl, deleteUrl, replyUrl) {
        $("#jq-table").DataTable({
            responsive: true,
            // Design Assets
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            // Searching Setups
            searching: { regex: true },
            // Ajax Filter
            ajax: {
                url: apiUrl,
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: function (d) {
                    return JSON.stringify(d);
                }
            },
            // Columns Setups
            columns: [
                { data: "userFullName" },
                { data: "postTitle" },
                {
                    data: "body",
                    render: function (data, type, row) {
                        return reduceTextToSmallerWidth(row.body, 50);
                    }
                },
                { data: "email" },
                {
                    data: "status",
                    render: function (data, type, row) {
                        switch (row.status) {
                            case 1000:
                                return "Unclear";
                            case 2000:
                                return "Rejected";
                            case 3000:
                                return "Accepted";
                            default:
                                return "";
                        }
                    }
                },
                { data: "ip" },
                {
                    data: "isEdited",
                    render: function (data, type, row) {
                        return boolToText(row.isEdited);
                    }
                },
                {
                    data: "createDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.createDate);
                    }
                },
                {
                    data: "lastEditDate",
                    render: function (data, type, row) {
                        return Panel.DataTableRenderDate(row.lastEditDate);
                    }
                },
                {
                    mRender: function (data, type, row) {
                        var linkReply = `<a href="${replyUrl}/-1" class="btn btn-success" title="Reply"><i class="fa fa-comment"></i></a>`;
                        linkReply = linkReply.replace("-1", row.id);

                        return linkReply + " | " + renderButtons(editUrl, detailsUrl, deleteUrl, row.id);
                    }
                }
            ],
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: false },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }

                        return data;
                    }
                },
                { targets: "date-type", type: "date-eu" }
            ]
        });
    }

    //===================================
    //            Text editor
    //===================================
    var initializeEditor = function bindEditor(elementId) {
        let element;

        if (elementId === undefined) {
            element = $("#txtContent");
        } else {
            element = $(elementId);
        }

        element.ckeditor({
            language: 'en',
            extraPlugins: 'wordcount'
        });
    }

    var initializeEditorReadOnly = function bindEditorReadOnly(elementId) {
        let element;

        if (elementId === undefined) {
            element = $("#txtContent");
        } else {
            element = $(elementId);
        }

        element.ckeditor({
            readOnly: true,
            language: 'en',
            extraPlugins: 'wordcount'
        });
    }

    //===================================
    //           Files Preview
    //===================================
    var initializeFilesPreview = function filePreview() {

        window.onload = function () {
            //Check File API support
            if (window.File && window.FileList && window.FileReader) {
                var filesInput = document.getElementById("input-files-hidden");
                filesInput.addEventListener("change",
                    function (event) {
                        var files = event.target.files; //FileList object
                        var output = document.getElementById("files-preview");
                        output.innerHTML = "";
                        for (var i = 0; i < files.length; i++) {
                            var file = files[i];
                            //Only pics
                            if (!file.type.match("image"))
                                continue;
                            var picReader = new FileReader();
                            picReader.addEventListener("load",
                                function (event) {
                                    var picFile = event.target;
                                    var img = document.createElement("IMG");
                                    img.setAttribute("class", "thumbnail");
                                    img.setAttribute("src", `${picFile.result}`);

                                    output.appendChild(img);
                                });
                            //Read the image
                            picReader.readAsDataURL(file);
                        }
                    });
            } else {
                console.log("Your browser does not support File API");
            }
        }
    }

    var initializeSingleFilePreview = function singleFilePreview() {
        window.onload = function () {
            //Check File API support
            if (window.File && window.FileList && window.FileReader) {
                var filesInput = document.getElementById("input-file-hidden");
                filesInput.addEventListener("change",
                    function (event) {
                        var files = event.target.files; //FileList object
                        var output = document.getElementById("files-preview");
                        output.innerHTML = "";
                        for (var i = 0; i < files.length; i++) {
                            var file = files[i];
                            //Only pics
                            if (!file.type.match("image"))
                                continue;
                            var picReader = new FileReader();
                            picReader.addEventListener("load",
                                function (event) {
                                    var picFile = event.target;
                                    var img = document.createElement("IMG");
                                    img.setAttribute("class", "thumbnail");
                                    img.setAttribute("src", `${picFile.result}`);

                                    output.appendChild(img);
                                });
                            //Read the image
                            picReader.readAsDataURL(file);
                        }
                    });
            } else {
                console.log("Your browser does not support File API");
            }
        }
    }

    //===================================
    //             Tag Input
    //===================================
    var initializeTagsInput = function tagsInput() {
        tagger(document.getElementById("tags-input"),
            {
                allow_duplicates: false,
                allow_spaces: true,
                link: function () {
                    return false;
                }
            });
    }

    var initializeTagsInputReadOnly = function tagsInputReadOnly() {
        tagger(document.getElementById("tags-input"),
            {
                allow_duplicates: false,
                allow_spaces: true,
                link: function () {
                    return false;
                }
            });

        $('.tagger').find('a.close').each(function () {
            $(this).remove();
        });

        $('.tagger').find('li.tagger-new').remove();

        $('.tagger').addClass('disabled');
    }

    //===================================
    //            Date Convertor
    //===================================
    var dataTableRenderDate = function dtRenderDate(utcDate) {
        var date = moment(utcDate).format('YYYY/MM/DD HH:mm:ss');
        let time = moment.utc(utcDate).local().format('HH:mm:ss');

        return time + " " + moment(date, 'YYYY/MM/DD HH:mm:ss').format('YYYY/MM/DD');
    }

    //===================================
    //            Sort DOM
    //===================================
    var initializeSortDom = function sortDom(sortableContainerElementId, sortableItemClassName) {

        return new Sortable(document.getElementById(sortableContainerElementId),
            {
                animation: 150,
                handle: '.handle',
                onUpdate: function (evt) {
                    [].forEach.call(evt.from.getElementsByClassName(sortableItemClassName),
                        function (el, index) {
                            el.querySelector(".sort-index").value = index;
                        });
                }
            });
    }


    return {
        LoadCertificatesTable: loadCertificatesTable,

        LoadContactsTable: loadContactsTable,

        LoadUsersTable: loadUsersTable,

        LoadPostsTable: loadPostsTable,
        LoadPostCategoriesTable: loadPostCategoriesTable,
        LoadPostCommentsTable: loadPostCommentsTable,

        InitializeEditor: initializeEditor,
        InitializeEditorReadOnly: initializeEditorReadOnly,

        InitializeFilesPreview: initializeFilesPreview,
        InitializeSingleFilePreview: initializeSingleFilePreview,

        InitializeTagsInput: initializeTagsInput,
        InitializeTagsInputReadOnly: initializeTagsInputReadOnly,

        DataTableRenderDate: dataTableRenderDate,

        InitializeSortDom: initializeSortDom
    }

})();
