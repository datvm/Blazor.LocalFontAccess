export function beforeStart() {
    const g = globalThis;
    const permName = "local-fonts";
    const permDenied = "PERMISSION_DENIED";

    g.blazorLocalFont ??= new class {

        isSupported() {
            return Boolean(g.queryLocalFonts);
        }

        async #getPermAsync() {
            return await navigator.permissions.query({
                name: permName,
            });
        }

        async getPermissionStateAsync() {
            const perm = await this.#getPermAsync();
            return perm.state;
        }

        async getFontsAsync(keepReference) {
            if (!this.isSupported) {
                throw new Error("NOT_SUPPORTED");
            }

            const perm = await this.#getPermAsync();
            if (perm.state === "denied") {
                throw new Error(permDenied);
            }

            const fonts = await new Promise(async (r, rej) => {
                try {
                    var fonts = await queryLocalFonts();

                    if (perm.state !== "granted") {
                        rej(permDenied);
                    }

                    r(fonts);
                } catch (e) {
                    rej(perm.state === "denied" ? permDenied : e.message);
                }
            });

            if (keepReference) {
                return fonts;
            } else {
                // Currently FontData objects cannot be serialized
                return fonts.map(q => ({
                    family: q.family,
                    fullName: q.fullName,
                    postscriptName: q.postscriptName,
                    style: q.style,
                }));    
            }
        }
    }();
}