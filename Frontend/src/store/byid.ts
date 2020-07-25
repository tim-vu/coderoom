export interface ById<T> {
  [key: string]: T;
}

export function toById<T>(array: T[], keySelector: (t: T) => string): ById<T> {
  return array.reduce((acc: ById<T>, curr) => {
    const key = keySelector(curr);
    acc[key] = curr;
    return acc;
  }, {});
}
