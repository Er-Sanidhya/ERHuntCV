$(document).ready(function () {

    $('.master-data-table').each(function () {

        var tableElement = $(this);

        var tableId =
            tableElement.attr('id');


        // =====================================================
        // PAGE INFO ID
        // =====================================================

        var pageInfoId = tableId
            ? tableId.replace('Table', 'PageInfo')
            : '';


        // =====================================================
        // COUNT TABLE COLUMNS
        // =====================================================

        var columnCount =
            tableElement
                .find('thead th')
                .length;


        // =====================================================
        // HORIZONTAL SCROLL
        // MORE THAN 5 COLUMNS
        // =====================================================

        var needsHorizontalScroll =
            columnCount > 5;


        // =====================================================
        // MASTER TABLE CONTAINER
        // =====================================================

        var container =
            tableElement.closest(
                '.master-table-container'
            );


        if (needsHorizontalScroll) {

            container.addClass(
                'master-table-scrollable'
            );

        }
        else {

            container.removeClass(
                'master-table-scrollable'
            );
        }


        // =====================================================
        // DATATABLE
        // =====================================================

        var masterTable =
            tableElement.DataTable({

                pageLength: 5,

                lengthMenu: [
                    [5, 10, 25, 50, 100],
                    [5, 10, 25, 50, 100]
                ],

                order: [],

                searching: true,

                paging: true,

                info: true,

                scrollX:
                    needsHorizontalScroll,

                autoWidth: false,

                pagingType:
                    "simple_numbers",

                language: {

                    search: "Search:",

                    lengthMenu:
                        "Show _MENU_ entries",

                    info:
                        "Showing _START_ to _END_ of _TOTAL_ records",

                    infoEmpty:
                        "Showing 0 to 0 of 0 records",

                    zeroRecords:
                        "No matching records found",

                    emptyTable:
                        "No records found",

                    paginate: {

                        next: "Next",

                        previous: "Previous"
                    }
                },


                // =================================================
                // INIT COMPLETE
                // =================================================

                initComplete: function () {

                    var api =
                        this.api();

                    updateSerialNumbers(api);

                    setTimeout(function () {

                        api.columns.adjust();

                    }, 50);
                }

            });


        // =====================================================
        // SERIAL NUMBER
        // =====================================================

        function updateSerialNumbers(api) {

            var pageInfo =
                api.page.info();


            var start =
                pageInfo.start;


            api.rows({
                page: 'current'
            }).every(function (rowIndex, tableLoop, rowLoop) {

                $(this.node())
                    .find('td.master-sr-no')
                    .text(start + rowLoop + 1);

            });
        }


        // =====================================================
        // PAGE X OF Y
        // =====================================================

        function updatePageInfo() {

            if (!pageInfoId) {

                return;
            }


            var pageInfo =
                masterTable.page.info();


            var currentPage =
                pageInfo.pages > 0
                    ? pageInfo.page + 1
                    : 0;


            var totalPages =
                pageInfo.pages;


            $('#' + pageInfoId).text(

                'Page ' +
                currentPage +
                ' of ' +
                totalPages

            );
        }


        updatePageInfo();


        // =====================================================
        // AFTER DRAW
        // =====================================================

        masterTable.on(
            'draw',
            function () {

                updatePageInfo();

                updateSerialNumbers(masterTable);


                setTimeout(function () {

                    masterTable
                        .columns
                        .adjust();

                }, 20);

            }
        );


        // =====================================================
        // WINDOW RESIZE
        // =====================================================

        $(window).on(

            'resize.masterTable_' +
            tableId,

            function () {

                setTimeout(function () {

                    masterTable
                        .columns
                        .adjust();

                }, 50);

            }

        );

    });

});