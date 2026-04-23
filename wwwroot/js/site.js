document.addEventListener("DOMContentLoaded", () => {
	const payrollForms = document.querySelectorAll("[data-payroll-form]");

	if (!payrollForms.length) {
		return;
	}

	const currencyFormatter = new Intl.NumberFormat("en-US", {
		style: "currency",
		currency: "USD"
	});

	const parseNumber = (value) => {
		if (typeof value !== "string") {
			return 0;
		}

		const normalized = value.replace(/,/g, "").trim();
		if (normalized === "") {
			return 0;
		}

		const parsed = Number.parseFloat(normalized);
		return Number.isFinite(parsed) ? parsed : 0;
	};

	const refreshPayrollPreview = (form) => {
		const dailyRate = parseNumber(form.dataset.dailyRate ?? "0");
		const daysWorkedInput = form.querySelector("[data-payroll-days-worked]");
		const deductionInput = form.querySelector("[data-payroll-deduction]");
		const grossPayInput = form.querySelector("[data-payroll-gross-pay]");
		const netPayInput = form.querySelector("[data-payroll-net-pay]");

		if (!daysWorkedInput || !deductionInput || !grossPayInput || !netPayInput) {
			return;
		}

		const daysWorked = Math.max(parseNumber(daysWorkedInput.value), 0);
		const deduction = Math.max(parseNumber(deductionInput.value), 0);
		const grossPay = daysWorked * dailyRate;
		const netPay = grossPay - deduction;

		grossPayInput.value = currencyFormatter.format(grossPay);
		netPayInput.value = currencyFormatter.format(netPay);
	};

	payrollForms.forEach((form) => {
		const daysWorkedInput = form.querySelector("[data-payroll-days-worked]");
		const deductionInput = form.querySelector("[data-payroll-deduction]");

		if (!daysWorkedInput || !deductionInput) {
			return;
		}

		const updatePreview = () => refreshPayrollPreview(form);

		daysWorkedInput.addEventListener("input", updatePreview);
		deductionInput.addEventListener("input", updatePreview);
		updatePreview();
	});
});
