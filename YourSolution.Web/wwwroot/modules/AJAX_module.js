import axios from 'axios';

export var defaultOption = {
    showLoading: true,
    showDialog: false,
    successTitle: '執行成功',
    errorTitle: '執行失敗'
};

/**
 * 取得頁面上的XSRF token，並刷新cookie["XSRF-TOKEN"]的內容
 * 需在頁面(*.cshtml)上加上以下2段程式碼
 * (C#) var requestToken = Antiforgery.GetAndStoreTokens(Context).RequestToken;
 * (HTML) <input id="RequestVerificationToken" type="hidden" value="@requestToken" />
 * @param {any} requestToken XSRF token
 */
function RefreshRequestToken() {
    let requestToken = document.getElementById("RequestVerificationToken");

    if (requestToken)
        document.cookie = `XSRF-TOKEN=${requestToken.value}`;
}

/**
 * HttpGet
 * @param {any} url 網址
 * @param {any} params 要傳遞的參數值，設定方式如下：
 * //URL参數放在params屬性裏面
    params: {
        gender: 'female',
        nat: 'us'
    }
 * @param {any} option 設定是否要跳出彈出視窗或顯示loading遮罩
 * @returns
 */
export async function HttpGetAsync(url, params = {}, option = null) {
    if (option == null)
        return await CommonHttpRequest(() => {
            return axios.get(url, {
                params: params,
                withCredentials: true
            });
        });

    return await CommonHttpRequest(() => {
        return axios.get(url, {
            params: params,
            withCredentials: true
        });
    }, option);
}

/**
 * HttpPost(傳遞FormBody)
 * @param {any} url 網址
 * @param {any} data 要傳遞的資料(object)
 * @param {any} option 設定是否要跳出彈出視窗或顯示loading遮罩
 * @returns
 */
export async function HttpPostAsync(url, data, option = null) {

    if (option == null)
        return await CommonHttpRequest(() => {
            return axios.post(url, data, {
                withCredentials: true
            })
        });

    return await CommonHttpRequest(() => {
        return axios.post(url, data, {
            withCredentials: true
        })
    }, option);
}

/**
 * HttpPost(傳遞FormBody 或 上傳檔案(二進位檔))
 * @param {any} url 網址
 * @param {any} data 要傳遞的資料(object)
 * @param {any} option 設定是否要跳出彈出視窗或顯示loading遮罩
 * @param {any} isUploadFile 是否要上傳檔案
 * @returns
 */
export async function HttpPostByFormDataAsync(url, data, option = null, isUploadFile = false) {

    let config = {
        withCredentials: true,
        headers: {
            "Content-Type": isUploadFile ? "multipart/form-data" :"application/x-www-form-urlencoded"
        }
    };

    if (option == null)
        return await CommonHttpRequest(() => {
            return axios.post(url, data, config)
        });

    return await CommonHttpRequest(() => {
        return axios.post(url, data, config)
    }, option);
}

/**
 * HTTP Request共用函式
 * @param {any} callback 呼叫的Http method
 * @param {any} option 設定是否要跳出彈出視窗或顯示loading遮罩
 * @returns
 */
async function CommonHttpRequest(callback, option = {
    showLoading: true,
    showDialog: false,
    successTitle: '執行成功',
    errorTitle: '執行失敗'
}) {
    /**預設API回傳的內容格式 */
    let returnData = {
        isOK: false,
        message: '',
        data: null,
        statusCode: 0 //Http StatusCode
    };

    RefreshRequestToken();

    if (option.showLoading)
        $.LoadingOverlay("show");

    try {
        const response = await callback();

        if (option.showLoading)
            $.LoadingOverlay("hide");

        if (response.status === 200) {
            returnData = response.data;
            returnData.statusCode = response.status;
            if (returnData.isOK) {
                if (option.showDialog) {
                    Swal.fire({
                        icon: "success",
                        title: option.successTitle,
                        text: returnData.message,
                        confirmButtonText: '確定',
                        didOpen: () => {
                            const swalContainer = document.querySelector('.swal2-container');
                            if (swalContainer) {
                                // 設定一個較高的 z-index
                                swalContainer.style.zIndex = '999999';
                            }
                        }
                    });
                }
            } else {
                if (option.showDialog) {
                    Swal.fire({
                        icon: "error",
                        title: option.errorTitle,
                        text: returnData.message,
                        confirmButtonText: '確定',
                        didOpen: () => {
                            const swalContainer = document.querySelector('.swal2-container');
                            if (swalContainer) {
                                // 設定一個較高的 z-index
                                swalContainer.style.zIndex = '999999';
                            }
                        }
                    });
                }
            }
        } else {
            returnData.isOK = true;
            returnData.statusCode = response.status;
        }
    } catch (error) {
        if (option.showLoading)
            $.LoadingOverlay("hide");

        let response = error.response;
        returnData.statusCode = response.status;

        if (response.status === 400) {
            //欄位驗證失敗，需顯示所有欄位的錯誤訊息

            let errorList = response.data.errors;

            const errorMessages = Object.entries(errorList)
                .map(([field, messages]) => {
                    //const name = field;
                    return `${messages.join(', ')}`;
                })
                .join('<br>');

            returnData.message = errorMessages;
            if (option.showDialog) {
                Swal.fire({
                    icon: "error",
                    title: option.errorTitle,
                    html: errorMessages,
                    confirmButtonText: '確定',
                    didOpen: () => {
                        const swalContainer = document.querySelector('.swal2-container');
                        if (swalContainer) {
                            // 設定一個較高的 z-index
                            swalContainer.style.zIndex = '999999';
                        }
                    }
                });
            }
        } else if (response.status === 401) {
            //console.log('需要 Windows 認證，請確認瀏覽器設定及網域信任。');
            location.replace('/Account/Login');
        } else if (response.status === 403) {
            //console.log('嘗試存取的頁面或資源需要特定的登入狀態或權限，而您目前的帳號不符合要求。');
            location.replace('/Account/Login');
        } else if (response.status === 409) {
            //會回傳409就代表目前顯示的資料已被其他人更新，需重新抓取資料，外部呼叫請用returnData.statusCode判斷
        } else {
            returnData.message = error.message;

            if (option.showDialog) {
                Swal.fire({
                    icon: "error",
                    title: option.errorTitle,
                    text: error.message,
                    confirmButtonText: '確定',
                    didOpen: () => {
                        const swalContainer = document.querySelector('.swal2-container');
                        if (swalContainer) {
                            // 設定一個較高的 z-index
                            swalContainer.style.zIndex = '999999';
                        }
                    }
                });
            }
        }
    } finally {
        return returnData;
    }
}

/**
 * 透過 HttpPost 下載檔案
 * @param {any} url
 * @param {any} fileName 預設下載檔名
 * @param {any} data
 */
export async function DownloadFileByPost(url, fileName, data) {
    try {
        // 顯示載入遮罩
        $.LoadingOverlay("show");

        const response = await axios.post(url, data, {
            responseType: 'blob',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        // 解析 Content-Disposition header 取得檔名
        const contentDisposition = response.headers['content-disposition'];
        if (contentDisposition) {
            const fileNameMatch = contentDisposition.match(/filename\*=UTF-8''(.+)|filename="(.+)"|filename=(.+)/);
            if (fileNameMatch) {
                fileName = decodeURIComponent(fileNameMatch[1] || fileNameMatch[2] || fileNameMatch[3]);
            }
        }

        // 建立 Blob URL 並觸發下載
        const blobUrl = window.URL.createObjectURL(new Blob([response.data]));
        const a = document.createElement('a');
        a.href = blobUrl;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(blobUrl);

    } catch (error) {
        // 嘗試讀取錯誤訊息
        let errorMessage = "發生錯誤，無法取得錯誤訊息";

        if (error.response && error.response.data) {
            try {
                // 將錯誤 blob 轉成文字
                const text = await error.response.data.text();
                errorMessage = text || errorMessage;
            } catch (e) {
                // 轉換失敗，維持預設錯誤訊息
            }
        } else if (error.message) {
            errorMessage = error.message;
        }

        Swal.fire({
            icon: "error",
            title: "錯誤",
            text: errorMessage,
            didOpen: () => {
                const swalContainer = document.querySelector('.swal2-container');
                if (swalContainer) {
                    // 設定一個較高的 z-index
                    swalContainer.style.zIndex = '999999';
                }
            }
        });
    } finally {
        // 隱藏載入遮罩
        $.LoadingOverlay("hide");
    }
}