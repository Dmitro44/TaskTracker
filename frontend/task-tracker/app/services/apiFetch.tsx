export async function apiFetch(input: RequestInfo, init?: RequestInit) {
    const res = await fetch(input, { ...init, credentials: "include" });
    console.log(res.status);
    if (res.status === 401 || res.status === 403) {
        if (typeof window !== "undefined") {
            window.location.href = "/login?from=" + encodeURIComponent(window.location.pathname);
        }
        throw new Error("Unauthorized");
    }
    return res;
}