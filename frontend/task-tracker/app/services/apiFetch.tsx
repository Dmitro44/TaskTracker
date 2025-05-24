export async function apiFetch(input: RequestInfo, init?: RequestInit) {
    const res = await fetch(input, { ...init, credentials: "include" });
    if (res.status === 401 || res.status === 403) {
        window.location.href = "/login?from=" + encodeURIComponent(window.location.pathname);
        throw new Error("Unauthorized");
    }
    return res;
}