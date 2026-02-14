import { ref, onMounted } from 'vue'
import { HttpGetAsync, HttpPostAsync, defaultOption, DownloadFileByPost } from '../modules/AJAX_module.js'

export default {
    setup() {
        /**分頁查詢條件 */
        const searchOption = {
            page: 1
        }

        const checkAll = ref(false);

        /**查詢結果 */
        const searchResult = ref([]);

        /**打卡地點選項 */
        const equNoOption = ref([]);

        /**回傳結果的頁碼設定 */
        const pageSetting = ref({
            page: 0,
            totalCount: 0,
            totalPage: 0,
            pageRangeList: []
        });

        onMounted(async () => {
            await GetequNoOption();
            await getSearchResult(1);
        });

        /**查詢上一頁 */
        async function PreviousSearch() {
            if (searchOption.page > 1)
                searchOption.page--;

            await getSearchResult(searchOption.page);
        }

        /**查詢下一頁 */
        async function NextSearch() {
            if (searchOption.page < pageSetting.value.totalPage)
                searchOption.page++;

            await getSearchResult(searchOption.page);
        }

        /**分頁查詢 */
        async function getSearchResult(page) {
            checkAll.value = false;
            searchOption.page = page;
            searchResult.value = [];
            pageSetting.value = {
                page: 0,
                totalCount: 0,
                totalPage: 0,
                pageRangeList: []
            };

            const settingOption = defaultOption;
            settingOption.showDialog = false;

            const response = await HttpPostAsync('/api/CardLogApi/GetCardLog', searchOption, settingOption);

            if (response.isOK) {
                searchResult.value = response.data.searchResult;

                searchResult.value.forEach((element, index) => {
                    element.selected = false;
                });

                searchOption.page = response.data.page;
                pageSetting.value = {
                    page: response.data.page,
                    totalCount: response.data.totalCount,
                    totalPage: response.data.totalPage,
                    pageRangeList: response.data.pageRangeList
                };
            } else {
                Swal.fire({
                    icon: "error",
                    title: '查詢失敗',
                    text: response.message,
                    confirmButtonText: '確定'
                });
            }
        }

        /**下載打卡紀錄的報表 */
        async function GetReportToExcel() {
            //const settingOption = defaultOption;
            //settingOption.showDialog = false;

            const response = await DownloadFileByPost('/api/CardLogApi/GetCardLogExportExcel', '', searchOption);
        }

        /**
         * 上傳打卡紀錄
         * @returns
         */
        async function UploadToWebITR() {
            let searchDto = searchResult.value.filter(x => x.selected);
            let recordIDArray = searchDto.map(x => x.recordID);

            if (recordIDArray.length <= 0) {
                Swal.fire({
                    icon: "error",
                    title: '錯誤',
                    text: '請指定要上傳的打卡紀錄',
                    confirmButtonText: '確定'
                });
                return;
            }

            Swal.fire({
                title: "確定要重新上傳打卡紀錄?",
                text: "將目前勾選的打卡紀錄，重新上傳至WebITR",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "是",
                cancelButtonText: "否"
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const settingOption = defaultOption;
                    settingOption.showDialog = true;
                    settingOption.successTitle = '同步成功';
                    settingOption.errorTitle = '同步失敗';

                    const response = await HttpPostAsync('/api/CardLogApi/SyncCardLogToWebITR', recordIDArray, settingOption);
                    await getSearchResult(1);
                    checkAll.value = false;
                }
            });
        }

        async function UploadToWebITRBySearchDto() {
            Swal.fire({
                title: "確定要重新上傳打卡紀錄?",
                text: "依據目前查詢條件，重新上傳打卡紀錄至WebITR",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "是",
                cancelButtonText: "否"
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const settingOption = defaultOption;
                    settingOption.showDialog = true;
                    settingOption.successTitle = '同步成功';
                    settingOption.errorTitle = '同步失敗';
                    const response = await HttpPostAsync('/api/CardLogApi/SyncCardLogToWebITRBySearchDto', searchOption, settingOption);
                    await getSearchResult(1);
                    checkAll.value = false;
                }
            });
        }

        async function GetequNoOption() {
            equNoOption.value = [];
            const response = await HttpGetAsync('/api/CardLogApi/GetEquNoOption', null);

            if (response.isOK) {
                equNoOption.value = response.data;
                equNoOption.value.unshift({equNo:'',equName:'全部地點'});
            }
        }

        /**更換打卡別 */
        async function ChangeLogStatus(logStatus, recordID) {
            Swal.fire({
                title: "確定要更換打卡別?",
                text: "",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "是",
                cancelButtonText: "否"
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const settingOption = defaultOption;
                    settingOption.showDialog = true;
                    settingOption.successTitle = '更換成功';
                    settingOption.errorTitle = '更換失敗';

                    let dto = {
                        logStatus: logStatus,
                        recordID: recordID
                    }

                    const response = await HttpPostAsync('/api/CardLogApi/ChangeLogStatus', dto, settingOption);
                    await getSearchResult(1);
                    checkAll.value = false;
                }
            });
        }

        function checkAllToggle() {
            checkAll.value = !checkAll.value;
            searchResult.value.forEach((element, index) => {
                if (element.isSyncToWebITR == false)
                    element.selected = checkAll.value;
            });
        }

        return { searchOption, searchResult, pageSetting, getSearchResult, PreviousSearch, NextSearch, UploadToWebITR, checkAllToggle, GetReportToExcel, UploadToWebITRBySearchDto, equNoOption, ChangeLogStatus };
    }
}