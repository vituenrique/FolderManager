function FolderManagerController($scope, $filter, $http, $location, $cacheFactory, $timeout, Global, $element) {

    $scope.FolderTreeJSON = undefined;
    $scope.hasChanges = false;
    $scope.treeID = "folderTree";

    // Create a tree view structure for folder management
    function CreateTree(data) {
        $('#' + $scope.treeID).jstree({
            checkbox: {
                tie_selection: false,
                whole_node: false
            },
            'core': {
                'themes': {
                    'name': 'proton',
                    'responsive': true
                },
                'multiple': false,
                'data': data,
                "dblclick_toggle": false,
                'check_callback': true
            },
            'plugins': ["themes", "html_data", "ui", "crrm", "contextmenu"],
            contextmenu: {
                items: customMenu
            }
        });
    }

    // Settings for the context menu
    function customMenu(node) {
        var tree = $("#" + $scope.treeID).jstree(true);
        var items = {
            "create": false,
            "rename": false,
            "ccp": false,
            "remove": false,
            
        };

        items.createItem =  { 
            label: "Create",
            action: function (data) {
                var inst = $.jstree.reference(data.reference),
                obj = inst.get_node(data.reference);
                inst.create_node(obj, { text: 'New File', type: 'file', icon: 'fa fa-folder' }, "last", function (new_node) {
                    new_node.data = { file: true };
                    setTimeout(function () { inst.edit(new_node); }, 0);
                });
                $scope.hasChanges = true;
                $scope.$apply();
            }
        };
        if (node.id != "root"){ // The root cannot be renamed or removed
            items.renameItem = {
                label: "Rename",
                action: function () {
                    tree.edit(node);
                    $scope.hasChanges = true;
                    $scope.$apply();
                }
            };
            items.deleteItem = {
                label: "Delete",
                action: function () {
                    tree.delete_node(node);
                    $scope.hasChanges = true;
                    $scope.$apply();
                }
            };
        }

        return items;
    }

    // Save the updated folders structure 
    $scope.UpdateData = function () {
        var currentTreeJson = $('#' + $scope.treeID).jstree(true).get_json('#', { flat: true });
        var jsonUpdated = JSON.stringify(currentTreeJson);

        Global.Requisition("/Folder/UpdateDB/", { json: jsonUpdated }, function (data) {
            var response = data.data;
            if (response.status) {
                $scope.hasChanges = false;
                Global.SendNotification(response.msg, 'success');
            } else {
                Global.SendNotification(response.msg, 'danger');
            }
        });
    }

    // Get the folder structure
    $scope.Get = function () {
        Global.Requisition("/Folder/Get/", {}, function (data) {
            var response = data.data;
            if (response.status) {
                $scope.folderTree = JSON.parse(response.response);
                CreateTree($scope.folderTree);
                Global.SendNotification(response.msg, 'success');
            } else {
                if (response.createNewJSON) { // Check if there's no base JSON to work with, so we have to create the root node.
                    var root_node = {
                        "id": "root",
                        "text": "root",
                        "icon": "fa fa-folder",
                        "li_attr": { "id": "root" },
                        "a_attr": { "href": "#", "class": "no_checkbox", "id": "root_anchor" },
                        "state": { "loaded": true, "opened": true, "selected": false, "disabled": false },
                        "data": {},
                        "parent": "#"
                    }
                    $scope.folderTree = root_node;
                    CreateTree($scope.folderTree);
                } else {
                    Global.SendNotification(response.msg, 'danger');
                }
            }
        });
    }
    
    // Function executed right after the first page load.
    function OnInit() {
        switch (FolderPage) {
            case 'Index':
                $scope.Get();
                break;
            default:
                break;
        }
    }
    OnInit();
}

app.controller("FolderManagerModule", FolderManagerController);