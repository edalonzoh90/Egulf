class Crypto  {
    static Encrypt(sin) {
        let key = CryptoJS.enc.Utf8.parse(Window.Key);
        let iv = CryptoJS.enc.Utf8.parse(Window.Iv);
        let encryptedVariable = sin;
        encryptedVariable = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(encryptedVariable), key,
            {
                keySize: 128 / 8,
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });
        return encryptedVariable.toString();
    }
}