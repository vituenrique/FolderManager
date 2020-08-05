function FolderManagerController($scope, $filter, $http, $location, $cacheFactory, $timeout, Global, $element) {

    $scope.FolderTreeJSON = undefined;
    $scope.hasChanges = false;
    $scope.treeID = "folderTree";

    $scope.Actions = [];

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
            'plugins': ["themes", "html_data", "ui", "crrm", "contextmenu", "search"],
            contextmenu: {
                items: customMenu
            }
        });

        $('#' + $scope.treeID).on('ready.jstree', function () { $(this).jstree('open_all') });

        var to = false;
        $('#search_bar').keyup(function () {
            if (to) { clearTimeout(to); }
            to = setTimeout(function () {
                var v = $('#search_bar').val();
                $('#' + $scope.treeID).jstree(true).search(v);
            }, 250);
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
                if (obj.id.includes("j")) {
                    Global.SendNotification("A new folder cannot be created inside that is yet to be saved.", "danger")
                    return;
                }
                inst.create_node(obj, { text: 'New Folder', type: 'file', icon: 'fa fa-folder', state: 'opened' }, "last", function (new_node) {
                    new_node.data = { file: true };
                    $scope.Actions.push({ action: "add", id: new_node.id, name: new_node.text, parent: new_node.parent });
                });
                $scope.hasChanges = true;
                $scope.$apply();
            }
        };
        if (node.parent != "#") {
            items.renameItem = {
                label: "Rename",
                action: function (data) {
                    var inst = $.jstree.reference(data.reference),
                    obj = inst.get_node(data.reference);
                    if (obj.id.includes("j")) {
                        Global.SendNotification("A new folder cannot be renamed before being saved.", "danger")
                        return;
                    }
                    tree.edit(node);
                    $scope.Actions.push({ action: "edit", id: node.id });
                    $scope.hasChanges = true;
                    $scope.$apply();
                }
            };

            items.deleteItem = {
                label: "Delete",
                action: function () {
                    tree.delete_node(node);
                    if (node.id.includes("j")) { // If the folder was just created but not saved do nothing.
                        for (var i = 0; i < $scope.Actions.length; i++) {
                            if ($scope.Actions[i].id == node.id) {
                                $scope.Actions.splice(i);
                            }
                        }
                    } else {
                        $scope.Actions.push({ action: "del", id: node.id });
                    }
                    $scope.hasChanges = true;
                    $scope.$apply();
                }
            };
        }
        
        return items;
    }

    // Save the updated folders structure 
    $scope.PerformeSave = function () {
        if ($scope.Actions <= 0) return;

        for (var i = 0; i < $scope.Actions.length; i++) {
            if ($scope.Actions[i].action == 'edit') {
                var node = $('#' + $scope.treeID).jstree(true).get_node($scope.Actions[i].id);
                $scope.Actions[i].name = node.text;
            }
        }

        Global.Requisition("/Folder/PerformeActions/", { actionJSON: JSON.stringify($scope.Actions) }, function (data) {
            var response = data.data;
            if (response.status) {
                document.location.reload(true);
                Global.SendNotification(response.msg, 'success');
            } else {
                Global.SendNotification(response.msg, 'danger');
            }
        });
    }

    // Get the folder structure
    $scope.GetFolders = function () {
        Global.Requisition("/Folder/GetFolders/", {}, function (data) {
            var response = data.data;
            if (response.status) {
                CreateTree(response.response)
                Global.SendNotification(response.msg, 'success');
            } else {
                Global.SendNotification(response.msg, 'danger');
            }
        });
    }
    
    // Function executed right after the first page load.
    function OnInit() {
        switch (FolderPage) {
            case 'Index':
                $scope.GetFolders();
                break;
            default:
                break;
        }
    }
    OnInit();
}

app.controller("FolderManagerModule", FolderManagerController);