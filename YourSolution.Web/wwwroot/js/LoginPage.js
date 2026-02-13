import { ref } from 'vue'
import { HttpGetAsync, HttpPostAsync, defaultOption } from '../modules/AJAX_module.js'

export default {
    setup() {
        const account = ref('');
        const pwd = ref('');

        /**本地帳號登入 */
        async function Login() {
            const settingOption = defaultOption;
            settingOption.showDialog = true;
            settingOption.successTitle = '登入成功';
            settingOption.errorTitle = '登入失敗';

            const response = await HttpPostAsync('/api/AccountApi/Login', {
                account: account.value,
                pwd: pwd.value
            }, settingOption);

            if (response.isOK) {
                location.replace('/Home/Index');
            }
        }

        /**網域登入 */
        async function WindowsLogin() {
            const settingOption = defaultOption;
            settingOption.showDialog = true;
            settingOption.successTitle = '登入成功';
            settingOption.errorTitle = '登入失敗';

            const response = await HttpGetAsync('/api/AccountApi/WindowsLogin', {}, settingOption);

            if (response.isOK) {

                // 取得網址的查詢字串部分
                const queryString = window.location.search;

                // 建立 URLSearchParams 物件
                //const urlParams = new URLSearchParams(queryString);
                //const ReturnUrl = urlParams.get('ReturnUrl');

                //if (urlParams && ReturnUrl) {
                //    location.replace(ReturnUrl);
                //} else {
                //    location.replace('/CardLog/Index');
                //}

                location.replace('/Home/Index');
            }
        }

        return { account, pwd, Login, WindowsLogin };
    }
}